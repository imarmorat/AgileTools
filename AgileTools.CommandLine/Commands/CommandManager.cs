using AgileTools.Analysers;
using AgileTools.CommandLine.Commands.Modifer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
{
    /// <summary>
    /// Takes care of command execution and notification
    /// </summary>
    public class CommandManager
    {
        #region Private

        private Context _context;
        private IEnumerable<ICommandModifierHandler> _modifierHandlers;

        #endregion

        public EventHandler<CmdExecutedEventArgs> OnCmdExecuted;

        public IEnumerable<ICommand> KnownCommands { get; set; }

        public CommandManager(Context context)
        {
            _context = context;
            _modifierHandlers = new List<ICommandModifierHandler>
            {
                new ExportCommandModifierHandler()
            };
        }

        public object ExecuteCommand(ICommand command, IList<string> parameters, ref IList<CommandError> errors)
        {
            //
            // check wether command postfix
            // format is "-> {destination} {format}
            var modifierHandler = (ICommandModifierHandler)null;
            IEnumerable<string> modifierParams = null;
            IEnumerable<string> commandParams = null;

            var modifers =
                from p in parameters
                from mh in _modifierHandlers
                where p == mh.ModifierKey
                select mh;

            if (modifers.Count() > 1)
            {
                errors.Add(new CommandError("command modifier", "more than one modifier found. expecting 0 or 1."));
                return null;
            }

            if (modifers.Count() == 1)
            {
                modifierHandler = modifers.ElementAt(0);
                var modifierIndex = parameters.IndexOf(modifierHandler.ModifierKey);
                modifierParams = parameters.Skip(modifierIndex + 1);
                commandParams = parameters.Take(modifierIndex).ToList();
            }
            else
                commandParams = parameters;

            var output = command.Run(_context, commandParams, ref errors);

            if (!errors.Any())
                modifierHandler?.Handle(modifierParams, output);
            else
                errors.Add(new CommandError("command modifiger", "Error found during command execution, command modifier skipped"));
            
            NotifyCmdExecuted(command, parameters);
            return output;
        }

        public object ExecuteFromString(string commandStr, ref IList<CommandError> errors)
        {
            //
            // 1. gather inputs from the string
            var commandQuery = Regex.Matches(commandStr, "[^\\s\"']+|\"([^\"]*)\"|'([^ ']*)\'");
            var commandName = string.Empty;
            var commandParams = new List<string>();
            foreach (Match match in commandQuery)
            {
                if (!match.Success)
                {
                    errors.Add(new CommandError("command line", $"Issue parsing argument {match.Index} (value: {match.Value})"));
                    return null;
                }

                if (match.Index == 0)
                    commandName = match.Value;
                else
                    commandParams.Add(match.Value);
            }
            
            //
            // 2. find associated command
            var command = KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
            if (command == null)
            {
                errors.Add(new CommandError("command line",  $"Command '{commandName}' unknown"));
                return null;
            }

            //
            // 3. execute command
            var output = ExecuteCommand(command, commandParams, ref errors);

            return output;
        }

        protected void NotifyCmdExecuted(ICommand cmd, IEnumerable<string> parameters)
        {
            var shadowCopy = OnCmdExecuted;
            if (shadowCopy != null)
                shadowCopy(this, new CmdExecutedEventArgs(cmd, parameters));
        }
    }

    /// <summary>
    /// Used to pass around the invoked command information
    /// </summary>
    public class CmdExecutedEventArgs : EventArgs
    {
        public ICommand Command { get; protected set; }
        public IEnumerable<string> Parameters { get; protected set; }

        public CmdExecutedEventArgs(ICommand cmd, IEnumerable<string> parameters)
        {
            Command = cmd;
            Parameters = parameters;
        }
    }
}
