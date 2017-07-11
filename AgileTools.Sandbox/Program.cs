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
            var jiraClient = new JiraClient("http://10.0.75.1:8080", "admin", "123", new DefaultModelConverter());
            var jiraService = new JiraService(jiraClient);
            jiraService.Init();

            _logger.Info("Start loading tickets");
            var tickets = jiraService.GetTickets("project = \"STP\"").ToList();
            _logger.Debug($"Found {tickets.Count} tickets");

            var analysers = new List<IAnalyser<object>>
            {
                new CumulativeFlowAnalyser(jiraService, tickets)
            };

            analysers.ForEach(a => a.Analyse());

            Console.ReadLine();
        }
    }
}
