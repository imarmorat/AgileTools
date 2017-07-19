using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
{
    public static class CommandExtension
    {
        public static string GetUsage(this ICommand command, GetCommandHelpCommand.Level level)
        {
            var sb = new StringBuilder();
            switch(level)
            {
                case GetCommandHelpCommand.Level.Summary:
                    sb.Append($"{command.CommandName} - {command.Description}");
                    break;

                case GetCommandHelpCommand.Level.Medium:
                    sb.Append($"{command.CommandName} - ");
                    command.Parameters.ForEach(p => sb.Append($"{p.Name} "));
                    break;

                case GetCommandHelpCommand.Level.Full:
                    sb.AppendLine($"{command.CommandName} - {command.Description}");
                    sb.AppendLine("Parameters:");
                    command.Parameters.ForEach(p => sb.AppendLine($"\t{p.Name} - {p.Description}"));
                    break;

                default:
                    throw new Exception($"Unhandled level {level}");
            }

            return sb.ToString();
        }
    }

}
