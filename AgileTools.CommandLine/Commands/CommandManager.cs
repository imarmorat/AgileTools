using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
{
    public class CommandManager
    {
        private Context _Context;

        public EventHandler<CmdExecutedEventArgs> OnCmdExecuted;

        public IEnumerable<ICommand> KnownCommands { get; set; }

        public CommandManager(Context context)
        {
            _Context = context;
        }

        public string ExecuteCommand(ICommand command, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var output = command.Run(_Context, parameters, ref errors);
            NotifyCmdExecuted(command, parameters);
            return output;
        }

        public string ExecuteFromString(string commandStr, ref IList<CommandError> errors)
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
            return ExecuteCommand(command, commandParams, ref errors);
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
