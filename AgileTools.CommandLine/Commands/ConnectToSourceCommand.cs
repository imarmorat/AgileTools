using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;

namespace AgileTools.CommandLine.Commands
{
    public class ConnectToSourceCommand : CommandBase
    {
        public override string CommandName => "connect";
        public override string Description => "connect to a source to retrieve cards, etc.";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("sourcetype", "e.g.: jira, rally", false),
            new CommandParameter.StringParameter("sourceparameter", "e.g.: jira, rally", false)
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var paramCount = parameters.Count();
            if (paramCount != ExpectedParameters.Count())
            {
                errors.Add(new CommandError("command parameters", "incorrect parameter count"));
                return null;
            }

            var sourceType = parameters.ElementAt(0).Trim();
            var sourceParameter = parameters.ElementAt(0).Trim();



            return $"Connected.";
        }
    }
}
