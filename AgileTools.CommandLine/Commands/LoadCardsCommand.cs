using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class LoadCardsCommand : ICommand
    {
        public string CommandName => "loadCards";

        public string GetUsage()
        {
            return $"{CommandName} \"query\" [clearCache=true] - loads card from source into the cache";
        }

        public string Run(Context context, IEnumerable<string> parameters)
        {
            var query = parameters.ElementAt(0).Trim('\"');
            var doClearCache = parameters.Count() == 2 ? Convert.ToBoolean(parameters.ElementAt(1)) : true;

            if (!doClearCache) throw new NotImplementedException("need to think about avoiding duplicate. removing old ones first?");

            var cards = context.JiraService.GetTickets(query);
            context.LoadedCards.Clear();
            foreach (var card in cards)
                context.LoadedCards.Add(card);

            return $"Loaded {context.LoadedCards.Count()} cards into cache.";
        }

        public bool TryParse(IEnumerable<string> parameters, out IList<ParameterError> paramErrors)
        {
            paramErrors = new List<ParameterError>();
            var paramCount = parameters.Count();
            if (paramCount < 1 || paramCount > 2)
                paramErrors.Add(new ParameterError("command parameters", "incorrect parameter count, expecting 1 or 2"));
            return !paramErrors.Any();
        }
    }
}
