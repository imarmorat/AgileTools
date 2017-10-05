using AgileTools.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Common.Commands
{
    /// <summary>
    /// Lets macro logic know that the command is not recordable in macro mode
    /// </summary>
    public interface IMacroNotRecordable { }

    /// <summary>
    /// Allows for macros to be recorded and executed.
    /// Known issues: macro name not checked whether it is fit to be stored into a file
    /// </summary>
    public class MacroCommand : CommandBase, IMacroNotRecordable
    {
        /// <summary>
        /// Macro data 
        /// </summary>
        public class Macro
        {
            public class MacroStep {  public string CommandName { get; set; } public IEnumerable<string> CommandArgs { get; set; } }

            public string Name { get; set; }
            public DateTime RecordedOn { get; set; }
            public IList<MacroStep> Steps { get; set; }

            public static Macro Load(string macroName)
            {
                var filename = $"macro - {macroName}.json";
                return LoadFile(filename);
            }

            public static Macro LoadFile(string filename)
            {
                if (!File.Exists(filename))
                    return null;

                var content = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<Macro>(content);
            }

            public static bool Delete(string macroName)
            {
                var filename = $"macro - {macroName}.json";
                if (!File.Exists(filename))
                    return false;

                File.Delete(filename);
                return true;
            }

            public void Save()
            {
                var filename = $"macro - {Name}.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public override string CommandName => "macro";
        public override string CommandGroup => "App";
        public override string Description => "allow for macros to be loaded ran recorded, etc.";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("action", "", false),
            new CommandParameter.StringParameter("action param", "", true),
        };

        public override CommandManager CommandManager
        {
            get
            {
                return _cmdManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _cmdManager = value;
                _cmdManager.OnCmdExecuted += (o, e) =>
                {
                    if (CurrentMode == MacroMode.Recording)
                    {
                        // We dont record macro commands. If concept of non recordable commands needs to be extended, add a flag in ICommand
                        if (e.Command is IMacroNotRecordable)
                            return;

                        CurrentMacro.Steps.Add(new Macro.MacroStep
                        {
                            CommandName = e.Command.CommandName,
                            CommandArgs = e.Parameters
                        });
                    }
                };
            }
        }

        #region private and protected

        protected enum MacroMode { Sleeping, Running, Recording }
        protected MacroMode CurrentMode = MacroMode.Sleeping;
        protected Macro CurrentMacro;
        private CommandManager _cmdManager;

        #endregion

        public MacroCommand()
        {
            
        }

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var action = (string)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var actionParam = parameters.Count() > 1 ?
                (string)ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1)) :
                string.Empty;

            switch (action.ToLower())
            {
                case "peek": return PeekMacro(actionParam);
                case "record": return RecordMacro(actionParam);
                case "run": return RunMacro(actionParam, context);
                case "save": return SaveMacro(context);
                case "cancel": return CancelMacro(context);
                case "status": return GetMacroStatus();
                case "list": return ListMacros(context);
                case "delete": return DeleteMacro(actionParam);

                default:
                    return new CommandOutput($"unknown action '{action}'", false);
            }
        }

        #region sub commands

        private CommandOutput ListMacros(Context context)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "macro - *.json");
            if (files.Count() == 0)
                return new CommandOutput("No macro found", false);

            var sb = new StringBuilder();
            files.ForEach(f =>
            {
                var macro = Macro.LoadFile(f);
                sb.AppendLine($"- {macro.Name} ({macro.RecordedOn}, {macro.Steps.Count()} steps)");
            });
            return new CommandOutput(sb.ToString(), true);
        }

        private CommandOutput DeleteMacro(string macroName)
        {
            var success = Macro.Delete(macroName);
            return new CommandOutput( success ? "Deleted." : $"failed to delete macro: either does not exist or cannot delete", success);
        }

        private CommandOutput GetMacroStatus()
        {
            return new CommandOutput(
                $"Mode is {CurrentMode}" + (CurrentMacro != null ? $", Current macro is {CurrentMacro.Name}" : ""),
                true);
        }

        private CommandOutput CancelMacro(Context context)
        {
            if (CurrentMode != MacroMode.Recording)
                return new CommandOutput($"cannot cancel as no macro recording", false);

            CurrentMacro = null;
            CurrentMode = MacroMode.Sleeping;
            return new CommandOutput($"Cancelled.", true);
        }

        private CommandOutput SaveMacro(Context context)
        {
            if (CurrentMode != MacroMode.Recording)
                return new CommandOutput($"cannot save as no macro recording", false);

            CurrentMacro.Save();
            CurrentMacro = null;
            CurrentMode = MacroMode.Sleeping;
            return new CommandOutput($"Saved.", true);
        }

        private CommandOutput RecordMacro(string macroName)
        {
            if (CurrentMode != MacroMode.Sleeping)
                return new CommandOutput($"cannot start recording as currently in mode '{CurrentMode}'", false);

            CurrentMacro = new Macro { Name = macroName, RecordedOn = DateTime.Now, Steps = new List<Macro.MacroStep>() };
            CurrentMode = MacroMode.Recording;
            return new CommandOutput($"Starting recording for macro {CurrentMacro.Name}", true);
        }

        private CommandOutput RunMacro(string macroName, Context context)
        {
            if (CurrentMode == MacroMode.Recording)
                return new CommandOutput($"cannot run a macro if recording", false);

            var macro = Macro.Load(macroName);
            if (macro == null)
                return new CommandOutput($"macro with name {macroName} not found", false);

            CurrentMode = MacroMode.Running;
            var output = new StringBuilder();
            output.AppendLine($"----- Macro '{macro.Name}' -----");
            try
            {
                macro.Steps.ForEach(step =>
                {
                    var cmd = _cmdManager.KnownCommands.FirstOrDefault(kc => kc.CommandName == step.CommandName);
                    if (cmd == null)
                        throw new Exception($"Command '{step.CommandName}' is unknown.Macro terminated.");

                    var result = _cmdManager.ExecuteCommand(cmd, step.CommandArgs.ToList());

                    output.AppendLine(result?.ToString());

                    // TODO: clean this up, not pretty
                    if (!result.IsSuccessful)
                        throw new Exception($"Errors while running");
                });
            }
            catch(Exception ex)
            {
                return new CommandOutput($"Macro execution encountered issues", ex, false);
            }
            finally
            {
                CurrentMode = MacroMode.Sleeping;
            }

            output.AppendLine("-------------------------------");
            return new CommandOutput( output.ToString(), true);
        }

        private CommandOutput PeekMacro(string macroName)
        {
            if (CurrentMode == MacroMode.Recording)
                return new CommandOutput($"cannot peek a macro if recording", false);

            var macro = Macro.Load(macroName);
            if (macro == null)
                return new CommandOutput($"macro with name {macroName} not found", false);

            var sb = new StringBuilder();
            sb.AppendLine($"Macro '{macro.Name}' created on {macro.RecordedOn} with steps:");
            macro.Steps.ForEach(s =>
            {
                sb.Append($"- {s.CommandName}");
                s.CommandArgs.ForEach(ca => sb.Append(" " + ca));
                sb.AppendLine();
            });
            return new CommandOutput(sb.ToString(), true);
        }

        #endregion
    }
}
