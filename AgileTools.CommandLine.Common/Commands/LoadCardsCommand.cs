using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;

namespace AgileTools.CommandLine.Common.Commands
{
    public class LoadCardsCommand : CommandBase
    {
        public override string CommandName => "loadCards";
        public override string CommandGroup => "Card Source";
        public override string Description => "load card from a file. Cache is cleared beforehand";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("filename", "file that contains the cards", false)
        };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var paramCount = parameters.Count();
            if (paramCount != 1)
                return new CommandOutput("Cannot execute - Incorrect parameter count", false);

            var filename = parameters.ElementAt(0).Trim();

            if (!File.Exists(filename))
                throw new Exception($"Cannot load cards as file {filename} not found");

            var content = File.ReadAllText(filename);
            var cards = JsonConvert.DeserializeObject<List<Card>>(content);

            context.LoadedCards.Clear();
            foreach(var card in cards)
                context.LoadedCards.Add(card);

            return new CommandOutput($"Fetched {context.LoadedCards.Count()} cards into cache.", true);
        }
    }
}
