using JiraTools.Client;
using JiraTools.Core.Models;
using JiraTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTools.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var jiraClient = new JiraClient("http://10.0.75.1:8080", "admin", "123", new DefaultModelConverter());
            var jiraService = new JiraService(jiraClient);
            jiraService.Init();

            var tickets = jiraService.GetTickets("project = \"STP\"").ToList();
        }
    }
}
