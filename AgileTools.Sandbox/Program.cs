using AgileTools.Client;
using AgileTools.Core.Models;
using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Analysers;
using log4net;

namespace AgileTools.Sandbox
{
    class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var jiraClient = (ICardManagerClient) new JiraClient("http://10.0.75.1:8080", "admin", "123");
            jiraClient = new CachedJiraClient(jiraClient);
            jiraClient.ModelConverter = new DefaultModelConverter(jiraClient);
            var jiraService = new JiraService(jiraClient);
            jiraService.Init();

            _logger.Info("Start loading tickets");
            var cards = jiraService.GetTickets("project = \"STP\"").ToList();
            _logger.Debug($"Found {cards.Count} tickets");

            var analysers = new List<IAnalyser<object>>
            {
                //new CumulativeFlowAnalyser(jiraService, cards),
                new RuleCheckerAnalyser  ( 
                    new List<RuleDefinitionBase> { new CardInProgressButNotAssignedRule() },
                    cards
                    ),
                new VelocityAnalyser(cards, DateTime.Parse("2017-07-05"), DateTime.Now.AddDays(3), new TimeSpan(1,0,0,0)),
            };

            analysers.ForEach(a =>
            {
                _logger.Info($"Starting [{a.Name}] analyser");
                var result = a.Analyse();
                AnalyserHelper.SaveAnalyserResult(a, result);
            });

            _logger.Info("All Done !!!");
            Console.ReadLine();
        }


    }
}
