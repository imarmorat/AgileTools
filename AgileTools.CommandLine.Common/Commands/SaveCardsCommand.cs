using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;

namespace AgileTools.CommandLine.Common.Commands
{
    public class SaveCardsCommand : CommandBase
    {
        public override string CommandName => "saveCards";
        public override string CommandGroup => "Card Source";
        public override string Description => "Save card into a file.";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("filename", "File where cards will be stored", false)
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var paramCount = parameters.Count();
            if (paramCount != 1)
            {
                errors.Add(new CommandError("command parameters", "incorrect parameter count, expecting 1"));
                return null;
            }

            var filename = parameters.ElementAt(0).Trim();
            var content = JsonConvert.SerializeObject(context.LoadedCards);
            File.WriteAllText(filename, content);

            return $"Fetched {context.LoadedCards.Count()} cards into cache.";
        }
    }
}
