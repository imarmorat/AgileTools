using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;

namespace AgileTools.CommandLine.Commands
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

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var paramCount = parameters.Count();
            if (paramCount != 1)
            {
                errors.Add(new CommandError("command parameters", "incorrect parameter count, expecting 1"));
                return null;
            }

            var filename = parameters.ElementAt(0).Trim();

            if (!File.Exists(filename))
                throw new Exception($"Cannot load cards as file {filename} not found");

            var content = File.ReadAllText(filename);
            var cards = JsonConvert.DeserializeObject<List<Card>>(content);

            context.LoadedCards.Clear();
            foreach(var card in cards)
                context.LoadedCards.Add(card);

            return $"Fetched {context.LoadedCards.Count()} cards into cache.";
        }
    }
}
