using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public abstract class RuleDefinitionBase
    {
        public virtual string Id { get; }
        public virtual string ShortName { get; }
        public virtual string Class { get; }
        public virtual string Description { get;  }

        public virtual IEnumerable<RuleBreach> RunCheck(IEnumerable<Card> cards)
        {
            return cards.Select(card => RunCheck(card)).Where(r=> r != null);
        }

        public abstract RuleBreach RunCheck(Card card);
    }

    public class CardInProgressButNotAssignedRule : RuleDefinitionBase
    {
        public override string Id => "R001";
        public override string ShortName => "CardInProgressButNotAssigned";
        public override string Class => "Sprint Inspection";
        public override string Description => "A card must be assigned to somebody if this is in progress for people to know who to talk to in needed";

        public override RuleBreach RunCheck(Card card)
        {
            if (card.Status.Category == StatusCategory.InProgress && card.Assignee == null)
                return new RuleBreach {
                    Rule = this,
                    Card = card,
                    Description = "No assignee found and is in progress",
                    Severity = RuleBreachSeverity.Critical };
            else
                return null;
        }
    }

    // other rules to implement:
    // wip limit
    // epic stories are all done but epic status not in final status
    // story subtasks not done while story is
    // no or too short AC for top x% of the backlog
    // flagged for more than x days
}
