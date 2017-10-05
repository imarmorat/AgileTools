using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AgileTools.Core;

namespace AgileTools.CommandLine.Common.Commands
{
    public enum HelpLevel { Summary = 's', Medium = 'd', Full = 'f' };

    public class GetCommandHelpCommand : CommandBase, IMacroNotRecordable
    {
        public override string CommandName => "help";
        public override string CommandGroup => "App";
        public override string Description => "gives help on available commands";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("command name", "command you need help on; if not specified, all commands are displayed", true),
        };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            if (parameters.Count() == 0)
            {
                var sb = new StringBuilder();

                var result =
                    from cmd in context.CmdManager.KnownCommands
                    group cmd by cmd.CommandGroup into grp
                    select new { GroupName = grp.Key, Commands = grp };

                result.ForEach(grp =>
                {
                    sb.AppendLine($"{grp.GroupName}");
                    grp.Commands.ForEach(cmd => sb.AppendLine($"\t{cmd.CommandName}: {cmd.Description}"));
                });

                //foreach (var cmd in context.CmdManager.KnownCommands)
                    //sb.AppendLine($"- {cmd.GetUsage(HelpLevel.Summary)}");

                return new CommandOutput(sb.ToString(), true); 
            }

            if (parameters.Count() == 1)
            {
                var commandName = parameters.ElementAt(0);
                var associatedCommand = context.CmdManager.KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
                return new CommandOutput(associatedCommand != null ? associatedCommand.GetUsage(HelpLevel.Full) : "unknwon command!", true);
            }

            return new CommandOutput("Too many parameters provided", false);
        }
    }
}
