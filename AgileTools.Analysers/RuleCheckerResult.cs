using System.Collections.Generic;
using System.Text;
using AgileTools.Core;
using System.Linq;

namespace AgileTools.Analysers
{
    public class RuleCheckerResult
    {
        public IEnumerable<RuleBreach> Breaches { get; set; }

        public override string ToString()
        {
            if (Breaches.Count() == 0)
                return "No breaches found!";

            var sb = new StringBuilder($"{Breaches.Count()} breaches found:");
            Breaches.ForEach(b => sb.AppendLine($"- [{b.Severity}] {b.Card.Id} - {b.Description} [{b.Rule.Id}]"));

            return sb.ToString();
        }
    }
}