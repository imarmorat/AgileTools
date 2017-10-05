using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;
using AgileTools.Core;

namespace AgileTools.CommandLine.Common.Commands
{
    public class ListSourceCommand : CommandBase
    {
        public override string CommandName => "listSrc";
        public override string CommandGroup => "Card Source";
        public override string Description => "list all card sources available";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
        };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            if (!context.AvailableCardServices.Any())
                return new CommandOutput("No source available!", false);

            var sb = new StringBuilder();
            foreach (var src in context.AvailableCardServices)
                sb.AppendLine($"\t- {src.Id}");

            return new CommandOutput(sb.ToString(), true);
        }
    }
}
