using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core;
using AgileTools.Analysers;
using AgileTools.Core.Models;

namespace AgileTools.CommandLine.Common.Commands
{
    /// <summary>
    /// Handler for velocity analyser
    /// </summary>
    public class VelocityAnalyserHandler : CommandBase
    {
        public override string CommandName => "velocity";
        public override string CommandGroup => "Analyser";
        public override string Description => "";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.DateTimeParameter("startDate", ""),
            new CommandParameter.DateTimeParameter("endDate", ""),
            new CommandParameter.IntParameter("bucketSize", "In days")
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() != 3)
            {
                errors.Add(new CommandError("parameter count", "expecting 3 parameters"));
                return null;
            }

            var startDate = (DateTime)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var endDate = (DateTime)ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1));
            var bucketSize = new TimeSpan((int)ExpectedParameters.ElementAt(2).Convert(parameters.ElementAt(2)), 0, 0, 0);

            var velocityAnalyser = new VelocityAnalyser(context.LoadedCards, startDate, endDate, bucketSize);
            return velocityAnalyser.Analyse().ToString();
        }
    }

    /// <summary>
    /// Handler for Rule Checker analyser
    /// </summary>
    public class BacklogRuleCheckerAnalyserHandler : CommandBase
    {
        public override string CommandName => "ruleChecker";
        public override string CommandGroup => "Analyser";
        public override string Description => "";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>();

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var rules = new List<RuleDefinitionBase>
            {
                new CardInProgressButNotAssignedRule()
            };

            var velocityAnalyser = new RuleCheckerAnalyser(rules, context.LoadedCards);

            return velocityAnalyser.Analyse().ToString();
        }
    }

    /// <summary>
    /// Handler for Cumulative Flow analyser
    /// </summary>
    public class CumulativeFlowAnalyserHandler : CommandBase
    {
        public override string CommandName => "cumulFlow";
        public override string CommandGroup => "Analyser";
        public override string Description => "Cumulative Flow analysis helps visualizing bottlenecks and cycle time";

        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.DateTimeParameter("startDate", ""),
            new CommandParameter.DateTimeParameter("endDate", ""),
            new CommandParameter.IntParameter("bucketSize", "In days"),
            new CommandParameter.StringParameter("statuses", "list of status to consider. if not specified, all statuses", true)
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() != 3)
            {
                errors.Add(new CommandError("parameter count", "expecting 3 parameters"));
                return null;
            }

            var startDate = (DateTime)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var endDate = (DateTime)ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1));
            var bucketSize = new TimeSpan((int)ExpectedParameters.ElementAt(2).Convert(parameters.ElementAt(2)), 0, 0, 0);
            var statusList = ExpectedParameters.Count() == 4 ?
                ExtractStatusList(context, (string)ExpectedParameters.ElementAt(3).Convert(parameters.ElementAt(3))) :
                null;

            var cmAnalyser = new CumulativeFlowAnalyser(context.CardService, context.LoadedCards, statusList, bucketSize, startDate, endDate);
            return cmAnalyser.Analyse();
        }

        private List<CardStatus> ExtractStatusList(Context context, string statusList)
        {
            var statuses = statusList.Split(',');
            return statuses.Select(sName => 
                context.CardService.GetStatuses()
                    .First(s => string.Compare(s.Name, sName, true) == 0)).ToList();
        }
    }

    /// <summary>
    /// Handler for Burndown analyser
    /// </summary>
    public class BurndownAnalyserHandler : CommandBase
    {
        public override string CommandName => "bdown";
        public override string CommandGroup => "Analyser";
        public override string Description => "Burndown analysis";

        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.DateTimeParameter("startDate", ""),
            new CommandParameter.DateTimeParameter("endDate", ""),
            new CommandParameter.DateTimeParameter("targetDate", ""),
            new CommandParameter.IntParameter("bucketSize", "In days"),
            new CommandParameter.IntParameter("minVelocity", "minimum velocity (confidence range)"),
            new CommandParameter.IntParameter("maxVelocity", "maximum velocity (confidence range)"),
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() != ExpectedParameters.Count())
            {
                errors.Add(new CommandError("parameter count", $"expecting {ExpectedParameters.Count()} parameters"));
                return null;
            }

            var startDate = (DateTime)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var endDate = (DateTime)ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1));
            var targetDate = (DateTime)ExpectedParameters.ElementAt(2).Convert(parameters.ElementAt(2));
            var bucketSize = new TimeSpan((int)ExpectedParameters.ElementAt(3).Convert(parameters.ElementAt(3)), 0, 0, 0);
            var minVelocity = (int)ExpectedParameters.ElementAt(4).Convert(parameters.ElementAt(4));
            var maxVelocity = (int)ExpectedParameters.ElementAt(5).Convert(parameters.ElementAt(5));

            var cmAnalyser = new BurndownAnalyser(context.LoadedCards, startDate, endDate, targetDate, bucketSize, minVelocity, maxVelocity);
            return cmAnalyser.Analyse();
        }
    }
}
