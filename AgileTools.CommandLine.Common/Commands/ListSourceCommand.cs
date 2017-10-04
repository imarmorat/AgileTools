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

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (!context.AvailableCardServices.Any())
                return "no source available";

            var sb = new StringBuilder();
            foreach (var src in context.AvailableCardServices)
                sb.AppendLine($"\t- {src.Id}");

            return sb.ToString() ;
        }
    }
}
