using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class GetCommandHelpCommand : ICommand
    {
        public string CommandName => "help";

        public string GetUsage()
        {
            return $"{CommandName} commandName - gives help on commands";
        }

        public string Run(Context context, IEnumerable<string> parameters)
        {
            if (parameters.Count() == 0)
            {
                var sb = new StringBuilder();
                foreach (var cmd in context.KnownCommands)
                    sb.AppendLine($"- {cmd.GetUsage()}");
                return sb.ToString(); 
            }

            var commandName = parameters.ElementAt(0);
            var associatedCommand = context.KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
            return associatedCommand != null ? associatedCommand.GetUsage() : "unknwon command!";
        }

        public bool TryParse(IEnumerable<string> parameters, out IList<ParameterError> paramErrors)
        {
            paramErrors = new List<ParameterError>();
            return true;
            //    paramErrors = new List<ParameterError>();
            //    if (parameters.Count() != 1)
            //        paramErrors.Add(new ParameterError("command name", "more than 1 parameter provided when expecting only 1"));
            //    return !paramErrors.Any();
        }
    }
}
