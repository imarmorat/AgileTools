using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ListCardsCommand : ICommand
    {
        public string CommandName => "listCards - lists cards that are in the cache";

        public string GetUsage()
        {
            return $"{CommandName}";
        }

        public string Run(Context context, IEnumerable<string> parameters)
        {
            if (context.LoadedCards.Count() == 0)
                return "No cards in cache";

            var sb = new StringBuilder();
            foreach (var card in context.LoadedCards)
                sb.AppendLine($"- {card}");
            return sb.ToString();
        }

        public bool TryParse(IEnumerable<string> parameters, out IList<ParameterError> paramErrors)
        {
            paramErrors = new List<ParameterError>();
            return true;
        }
    }
}
