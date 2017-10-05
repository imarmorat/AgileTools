using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;
using AgileTools.Core;

namespace AgileTools.CommandLine.Common.Commands
{
    public class ConnectToSourceCommand : CommandBase
    {
        public override string CommandName => "connect";
        public override string CommandGroup => "Card Source";
        public override string Description => "connect to a source to retrieve cards, etc.";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>
        {
            new CommandParameter.StringParameter("sourceid", "e.g.: jira, rally", false),
            //new CommandParameter.StringParameter("sourceparameter", "e.g.: jira, rally", false)
        };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var paramCount = parameters.Count();
            if (paramCount != ExpectedParameters.Count())
                return new CommandOutput("Cannot execute", new ArgumentException("incorrect parameter count"), false);

            var sourceId = parameters.ElementAt(0).Trim();
            var cardServiceConfig = context.AvailableCardServices.FirstOrDefault(p => p.Id == sourceId);
            if (cardServiceConfig == null)
                return new CommandOutput("Cannot load card service", new ArgumentException($"Service {sourceId} is unknown"), false);

            var cardServiceToUse = Utils.CreateSourceFromConfig(cardServiceConfig);

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

                    // not optimal, attribute could be added to the field but this is for later
                    var isItASecret = new List<string> { "password", "pwd", "passwd" }.Any(s => string.Compare(paramName, s, true) == 0);

                    var response = isItASecret ? Utils.ReadPasswordFromConsole() : Console.ReadLine();
                    initParams.Add(paramName, response);
                }
            }

            cardServiceToUse.Id = cardServiceConfig.Id;
            cardServiceToUse.Init(initParams);

            //
            // check connection 
            if (!cardServiceToUse.TryCheckConnection())
                return new CommandOutput("Connection to service failed", new Exception($"Connection to service failed. Check logs for more information"), false);

            context.CardService = cardServiceToUse;

            return new CommandOutput($"Connected.", true);
        }
    }
}
