using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Common.Commands
{
    public class ListCardsCommand : CommandBase
    {
        public override string CommandName => "listCards";
        public override string CommandGroup => "Card Source";
        public override string Description => "lists cards that are in the cache";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>();

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            if (context.LoadedCards.Count() == 0)
                return new CommandOutput("Cannot execute, no cards in cache", false);

            var sb = new StringBuilder();
            foreach (var card in context.LoadedCards)
                sb.AppendLine($"- {card}");

            return new CommandOutput(sb.ToString(), true);
        }
    }
}
