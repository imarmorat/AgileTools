using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class RuleCheckerAnalyser : IAnalyser<RuleCheckerResult>
    {
        private List<RuleDefinitionBase> _rules;
        private List<Card> _cards;

        public string Name { get => "Rule Checker Analyser";  }

        public RuleCheckerAnalyser(IEnumerable<RuleDefinitionBase> rules, IEnumerable<Card> cards)
        {
            _rules = new List<RuleDefinitionBase>(rules);
            _cards = new List<Card>(cards);
        }

        public RuleCheckerResult Analyse()
        {
            var result = new RuleCheckerResult
            {
                Breaches = _rules.SelectMany(r => r.RunCheck(_cards))
            };

            return result;
        }
    }
}
