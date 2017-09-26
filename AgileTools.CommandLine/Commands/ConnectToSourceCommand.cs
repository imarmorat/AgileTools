using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;
using AgileTools.Core;

namespace AgileTools.CommandLine.Commands
{
    public class ConnectToSourceCommand : CommandBase
    {
        public override string CommandName => "connect";
        public override string Description => "connect to a source to retrieve cards, etc.";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("sourceid", "e.g.: jira, rally", false),
            //new CommandParameter.StringParameter("sourceparameter", "e.g.: jira, rally", false)
        };

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var paramCount = parameters.Count();
            if (paramCount != ExpectedParameters.Count())
            {
                errors.Add(new CommandError("command parameters", "incorrect parameter count"));
                return null;
            }

            var sourceId = parameters.ElementAt(0).Trim();
            var cardServiceConfig = context.AvailableCardServices.FirstOrDefault( p=> p.Id == sourceId);
            if (cardServiceConfig == null)
            {
                errors.Add(new CommandError("Cannot load card service", $"Service {sourceId} is unknown"));
                return null;
            }

            var cardServiceToUse = Program.CreateSourceFromConfig(cardServiceConfig);

            //
            // if we have some missing parameters, we ask for it
            var initParams = new Dictionary<string, string>();
            foreach (var paramName in cardServiceToUse.InitParameters)
            {
                if (cardServiceConfig.Parameters.ContainsKey(paramName))
                    initParams.Add(paramName, cardServiceConfig.Parameters[paramName]);
                else
                {
                    Console.Write($"{paramName}: ");
                    var response = Console.ReadLine();
                    initParams.Add(paramName, response);
                }
            }

            cardServiceToUse.Id = cardServiceConfig.Id;
            cardServiceToUse.Init(initParams);

            //
            // check connection 
            if (!cardServiceToUse.TryCheckConnection())
            {
                errors.Add(new CommandError("Connection to service", $"Connection to service failed. Check logs for more information"));
                return "Connection failed";
            }

            context.CardService = cardServiceToUse;

            return $"Connected.";
        }
    }
}
