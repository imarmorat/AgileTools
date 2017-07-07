using System.Collections.Generic;

namespace AgileTools.Analysers
{
    public class RuleCheckerResult
    {
        public IEnumerable<RuleBreach> Breaches { get; set; }
    }
}