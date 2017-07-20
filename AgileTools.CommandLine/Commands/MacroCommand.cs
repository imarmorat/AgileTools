using AgileTools.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
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
        public override string Description => "allow for macros to be loaded ran recorded, etc.";
        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("action", "", false),
            new CommandParameter.StringParameter("action param", "", true),
        };

        #region private and protected

        protected enum MacroMode { Sleeping, Running, Recording }
        protected MacroMode CurrentMode = MacroMode.Sleeping;
        protected Macro CurrentMacro;
        private CommandManager _cmdManager;

        #endregion

        public MacroCommand(CommandManager cmdManager)
        {
            _cmdManager = cmdManager ?? throw new ArgumentNullException(nameof(cmdManager));
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

        public override string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var action = (string)Parameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var actionParam = parameters.Count() > 1 ?
                (string)Parameters.ElementAt(1).Convert(parameters.ElementAt(1)) :
                string.Empty;

            switch (action.ToLower())
            {
                case "peek": return PeekMacro(actionParam, errors);
                case "record": return RecordMacro(actionParam, errors);
                case "run": return RunMacro(actionParam, context, errors);
                case "save": return SaveMacro(context, errors);
                case "cancel": return CancelMacro(context, errors);
                case "status": return GetMacroStatus();
                case "list": return ListMacros(context, errors);
                case "delete": return DeleteMacro(actionParam, errors);

                default:
                    errors.Add(new CommandError("action", $"unknown action '{action}'"));
                    return null;
            }
        }

        #region sub commands

        private string ListMacros(Context context, IList<CommandError> errors)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "macro - *.json");
            if (files.Count() == 0)
                return "No macro found";

            var sb = new StringBuilder();
            files.ForEach(f =>
            {
                var macro = Macro.LoadFile(f);
                sb.AppendLine($"- {macro.Name} ({macro.RecordedOn}, {macro.Steps.Count()} steps)");
            });
            return sb.ToString();
        }

        private string DeleteMacro(string macroName, IList<CommandError> errors)
        {
            var success = Macro.Delete(macroName);
            if (!success)
            {
                errors.Add(new CommandError("delete", $"failed to delete macro: either does not exist or cannot delete"));
                return null;
            }

            return "Deleted.";
        }

        private string GetMacroStatus()
        {
            return $"Mode is {CurrentMode}" + (CurrentMacro != null ? $", Current macro is {CurrentMacro.Name}" : "");
        }

        private string CancelMacro(Context context, IList<CommandError> errors)
        {
            if (CurrentMode != MacroMode.Recording)
            {
                errors.Add(new CommandError("action", $"cannot cancel as no macro recording"));
                return null;
            }

            CurrentMacro = null;
            CurrentMode = MacroMode.Sleeping;
            return $"Cancelled.";
        }

        private string SaveMacro(Context context, IList<CommandError> errors)
        {
            if (CurrentMode != MacroMode.Recording)
            {
                errors.Add(new CommandError("action", $"cannot save as no macro recording"));
                return null;
            }

            CurrentMacro.Save();
            CurrentMacro = null;
            CurrentMode = MacroMode.Sleeping;
            return $"Saved.";
        }

        private string RecordMacro(string macroName, IList<CommandError> errors)
        {
            if (CurrentMode != MacroMode.Sleeping)
            {
                errors.Add(new CommandError("action", $"cannot start recording as currently in mode '{CurrentMode}'"));
                return null;
            }

            CurrentMacro = new Macro { Name = macroName, RecordedOn = DateTime.Now, Steps = new List<Macro.MacroStep>() };
            CurrentMode = MacroMode.Recording;
            return $"Starting recording for macro {CurrentMacro.Name}";
        }

        private string RunMacro(string macroName, Context context, IList<CommandError> errors)
        {
            if (CurrentMode == MacroMode.Recording)
            {
                errors.Add(new CommandError("action", $"cannot run a macro if recording"));
                return null;
            }

            var macro = Macro.Load(macroName);
            if (macro == null)
            {
                errors.Add(new CommandError("action", $"macro with name {macroName} not found"));
                return null;
            }

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

                    output.AppendLine(_cmdManager.ExecuteCommand(cmd, step.CommandArgs, ref errors));
                    if (errors.Any())
                        throw new Exception($"Errors while running");
                });
            }
            catch(Exception ex)
            {
                errors.Add(new CommandError("macro execution", ex.ToString()));
            }
            finally
            {
                CurrentMode = MacroMode.Sleeping;
            }

            output.AppendLine("-------------------------------");
            return output.ToString() ;
        }

        private string PeekMacro(string macroName, IList<CommandError> errors)
        {
            if (CurrentMode == MacroMode.Recording)
            {
                errors.Add(new CommandError("action", $"cannot peek a macro if recording"));
                return null;
            }

            var macro = Macro.Load(macroName);
            if (macro == null)
            {
                errors.Add(new CommandError("action", $"macro with name {macroName} not found"));
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Macro '{macro.Name}' created on {macro.RecordedOn} with steps:");
            macro.Steps.ForEach(s =>
            {
                sb.Append($"- {s.CommandName}");
                s.CommandArgs.ForEach(ca => sb.Append(" " + ca));
                sb.AppendLine();
            });
            return sb.ToString();
        }

        #endregion
    }
}
