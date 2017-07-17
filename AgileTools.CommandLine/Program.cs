using AgileTools.CommandLine.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core;
using AgileTools.Client;
using AgileTools.Core.Models;
using System.Text.RegularExpressions;

namespace AgileTools.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new Context
            {
                KnownCommands = new List<ICommand>
                {
                    new GetCommandHelpCommand(),
                    new LoadCardsCommand(),
                    new ListCardsCommand(),
                    new ExitCommand()
                },
                LoadedCards = new List<Card>()
            };

            PrintIntro();
            context.JiraService = InitCardManagerService();

            do
            {
                var (commandName, commandParams) = GetCommandLineFromUser();

                var command = context.KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
                if (command == null)
                {
                    Console.WriteLine($"Command '{commandName}' unknown");
                    continue;
                }

                if (!command.TryParse(commandParams, out IList<ParameterError> paramErrors))
                {
                    Console.WriteLine($"Command '{commandName}' has invalid arguments");
                    paramErrors.ForEach(pe => Console.WriteLine($"- {pe.ParameterName} : {pe.ErrorMessage}"));
                    continue;
                }

                var output = command.Run(context, commandParams);
                Console.WriteLine(output);

            } while (true);
        }

        /// <summary>
        /// Asks user for his input (command and its parameters)
        /// </summary>
        /// <returns></returns>
        private static (string commandName, List<string> commandParams) GetCommandLineFromUser()
        {
            Console.Write(":: ");
            var commandQuery = Regex.Matches(Console.ReadLine(), "[^\\s\"']+|\"([^\"]*)\"|'([^ ']*)\'");
            var commandName = string.Empty;
            var commandParams = new List<string>();
            foreach (Match match in commandQuery)
            {
                if (!match.Success)
                {
                    Console.WriteLine($"Issue parsing argument {match.Index} (value: {match.Value})");
                    continue;
                }

                if (match.Index == 0)
                    commandName = match.Value;
                else
                    commandParams.Add(match.Value);
            }
            return (commandName, commandParams);
        }

        private static JiraService InitCardManagerService()
        {
            Console.Write("user: ");
            var userName = Console.ReadLine();
            Console.Write("pwd: ");
            var pwd = Utils.ReadPasswordFromConsole();

            var jiraClient = (ICardManagerClient)new JiraClient("http://10.0.75.1:8080", userName, pwd);
            jiraClient = new CachedJiraClient(jiraClient);
            jiraClient.ModelConverter = new DefaultModelConverter(jiraClient);
            var jiraService = new JiraService(jiraClient);
            jiraService.Init();

            var user = jiraClient.GetUser(userName);
            Console.WriteLine($"\r\nWelcome {user.FullName} ({user.Id} / {user.Email})");

            return jiraService;
        }

        private static void PrintIntro()
        {
            Console.WriteLine("--- Agile Tools ---");
        }
    }
}
