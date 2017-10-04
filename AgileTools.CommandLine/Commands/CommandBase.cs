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
        /// <summary>
        /// Injected when command is autodiscovered
        /// </summary>
        public virtual CommandManager CommandManager { get; set; }

        public abstract string CommandName { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<CommandParameter> ExpectedParameters { get; }

        public abstract object Run(Context context, IEnumerable<string> rawParameters, ref IList<CommandError> errors);

        public virtual string GetUsage(HelpLevel level)
        {
            var sb = new StringBuilder();
            switch (level)
            {
                case HelpLevel.Summary:
                    sb.Append($"{CommandName} - {Description}");
                    break;

                case HelpLevel.Medium:
                    sb.Append($"{CommandName} - ");
                    ExpectedParameters.ForEach(p => sb.Append($"{p.Name} "));
                    break;

                case HelpLevel.Full:
                    sb.AppendLine($"{CommandName} - {Description}");
                    sb.AppendLine("Parameters:");
                    ExpectedParameters.ForEach(p => sb.AppendLine($"\t{p.Name} - {p.Description}"));
                    break;

                default:
                    throw new Exception($"Unhandled level {level}");
            }

            return sb.ToString();
        }
    }
}
