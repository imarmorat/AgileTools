using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ListCardsCommand : CommandBase
    {
        public override string CommandName => "listCards";
        public override string Description => "lists cards that are in the cache";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>();

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (context.LoadedCards.Count() == 0)
                return "No cards in cache";

            var sb = new StringBuilder();
            foreach (var card in context.LoadedCards)
                sb.AppendLine($"- {card}");
            return sb.ToString();
        }
    }
}
