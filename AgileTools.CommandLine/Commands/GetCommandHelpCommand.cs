﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class GetCommandHelpCommand : ICommand, IMacroNotRecordable
    {
        public string CommandName => "help";
        public string Description => "gives help on available commands";
        public enum Level {  Summary = 's', Medium = 'd', Full = 'f' };
        public IEnumerable<CommandParameter> Parameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("command name", "command you need help on; if not specified, all commands are displayed", true),
        };

        public string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() == 0)
            {
                var sb = new StringBuilder();
                foreach (var cmd in context.CmdManager.KnownCommands)
                    sb.AppendLine($"- {cmd.GetUsage(Level.Summary)}");
                return sb.ToString(); 
            }

            if (parameters.Count() == 1)
            {
                var commandName = parameters.ElementAt(0);
                var associatedCommand = context.CmdManager.KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
                return associatedCommand != null ? associatedCommand.GetUsage(Level.Full) : "unknwon command!";
            }

            errors.Add(new CommandError("parameter count", "too many parameters provided"));
            return null;
        }
    }
}