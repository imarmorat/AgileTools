using AgileTools.Core.Models;

namespace AgileTools.Analysers
{
    public enum RuleBreachSeverity {  Info, Warning, Critical }

    public class RuleBreach
    {
        public RuleDefinitionBase Rule { get; set; }
        public RuleBreachSeverity Severity { get; set; }
        public Card Card { get; set; }
        public string Description { get; set; }
    }
}