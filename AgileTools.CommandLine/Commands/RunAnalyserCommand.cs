using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core;
using AgileTools.Analysers;

namespace AgileTools.CommandLine.Commands
{
    public class RunAnalyserCommand : CommandBase
    {
        private IEnumerable<ICommand> _knownAnalysers = new List<ICommand>
        {
            new VelocityAnalyserHandler(),
            new BacklogRuleCheckerAnalyserHandler(),
            new CumulativeFlowAnalyserHandler()
        };

        public override string CommandName => "runAnalyser";
        public override string Description => "";
        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("analyser name", "", false)
        };

        public override string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() < 1)
            {
                errors.Add(new CommandError("analyser name", "name is mandatory"));
                return null;
            }

            var analyserToUse = parameters.ElementAt(0);
            var analyser = _knownAnalysers.FirstOrDefault(ka => ka.CommandName == analyserToUse);
            if (analyser == null)
            {
                errors.Add(new CommandError("analyser name", $"Analyser '{analyserToUse}' unknown. Use 'help' command to list available analysers"));
                return null;
            }

            return analyser.Run(context, parameters.Skip(1), ref errors);
        }

        public override string GetUsage(GetCommandHelpCommand.Level level)
        {
            var sb = new StringBuilder( base.GetUsage(level) );
            sb.AppendLine("*** Analysers ***");
            _knownAnalysers.ForEach(ka => sb.AppendLine($"\tAnalyser {ka.GetUsage(level)}"));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Handler for velocity analyser
    /// </summary>
    public class VelocityAnalyserHandler : CommandBase
    {
        public override string CommandName => "velocity";
        public override string Description => "";
        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>
        {
            new CommandParameter.DateTimeParameter("startDate", ""),
            new CommandParameter.DateTimeParameter("endDate", ""),
            new CommandParameter.IntParameter("bucketSize", "In days")
        };

        public override string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            if (parameters.Count() != 3)
            {
                errors.Add(new CommandError("parameter count", "expecting 3 parameters"));
                return null;
            }

            var startDate = (DateTime) Parameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var endDate = (DateTime) Parameters.ElementAt(1).Convert(parameters.ElementAt(1));
            var bucketSize = new TimeSpan((int)Parameters.ElementAt(2).Convert(parameters.ElementAt(2)), 0, 0, 0);

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
        public override string Description => "";
        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>();

        public override string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
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
        public override string Description => "Cumulative Flow analysis helps visualizing bottlenecks and cycle time";

        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>
        {
            new CommandParameter.DateTimeParameter("startDate", ""),
            new CommandParameter.DateTimeParameter("endDate", ""),
            new CommandParameter.IntParameter("bucketSize", "In days")
        };

        public override string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
                if (parameters.Count() != 3)
                {
                    errors.Add(new CommandError("parameter count", "expecting 3 parameters"));
                    return null;
                }

                var startDate = (DateTime)Parameters.ElementAt(0).Convert(parameters.ElementAt(0));
                var endDate = (DateTime)Parameters.ElementAt(1).Convert(parameters.ElementAt(1));
                var bucketSize = new TimeSpan((int)Parameters.ElementAt(2).Convert(parameters.ElementAt(2)), 0, 0, 0);

                var cmAnalyser = new CumulativeFlowAnalyser(context.JiraService, context.LoadedCards, bucketSize, startDate, endDate);
            return cmAnalyser.Analyse().ToString();
        }
    }
}
