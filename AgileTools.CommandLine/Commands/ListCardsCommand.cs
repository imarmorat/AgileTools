using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ListCardsCommand : ICommand
    {
        public string CommandName => "listCards";
        public string Description => "lists cards that are in the cache";
        public IEnumerable<CommandParameter> Parameters => new List<CommandParameter>();

        public string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
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
