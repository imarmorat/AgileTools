using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
{
    public abstract class CommandBase : ICommand
    {
        public abstract string CommandName { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<CommandParameter> Parameters { get; }

        public abstract object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors);

        public virtual string GetUsage(GetCommandHelpCommand.Level level)
        {
            var sb = new StringBuilder();
            switch (level)
            {
                case GetCommandHelpCommand.Level.Summary:
                    sb.Append($"{CommandName} - {Description}");
                    break;

                case GetCommandHelpCommand.Level.Medium:
                    sb.Append($"{CommandName} - ");
                    Parameters.ForEach(p => sb.Append($"{p.Name} "));
                    break;

                case GetCommandHelpCommand.Level.Full:
                    sb.AppendLine($"{CommandName} - {Description}");
                    sb.AppendLine("Parameters:");
                    Parameters.ForEach(p => sb.AppendLine($"\t{p.Name} - {p.Description}"));
                    break;

                default:
                    throw new Exception($"Unhandled level {level}");
            }

            return sb.ToString();
        }
    }
}
