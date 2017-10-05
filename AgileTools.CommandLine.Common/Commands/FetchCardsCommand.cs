using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Common.Commands
{
    public class FetchCardsCommand : CommandBase
    {
        public override string CommandName => "fetchCards";
        public override string CommandGroup => "Card Source";
        public override string Description => "fetch card from source into the cache (cache is cleared each time)";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("query", "must be compliant with card source manager", false)
        };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var paramCount = parameters.Count();
            if (paramCount != 1)
                return new CommandOutput("Cannot execute", new ArgumentException("incorrect parameter count"), false);

            var query = parameters.ElementAt(0).Trim('\"');

            var cards = context.CardService.GetTickets(query);
            context.LoadedCards.Clear();
            foreach (var card in cards)
                context.LoadedCards.Add(card);

            return new CommandOutput($"Fetched {context.LoadedCards.Count()} cards into cache.", true);
        }
    }
}
