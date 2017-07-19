﻿using AgileTools.CommandLine.Commands;
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
            var context = new Context();
            context.CmdManager = new CommandManager(context);
            context.CmdManager.KnownCommands = new List<ICommand>
                {
                    new GetCommandHelpCommand(),
                    new LoadCardsCommand(),
                    new ListCardsCommand(),
                    new ExitCommand(),
                    new RunAnalyserCommand(),
                    new MacroCommand(context.CmdManager) // I dont like my design, looks dodgy. change that later. context is passed around so should use it to access cmdmanager
                };

            PrintIntro();
            context.JiraService = InitCardManagerService();

            do
            {
                Console.Write(":: ");
                var cmdLine = Console.ReadLine();
                var errors = (IList<CommandError>)new List<CommandError>();
                var result = context.CmdManager.ExecuteFromString(cmdLine, ref errors);

                if (errors.Any())
                {
                    Console.WriteLine("Command failed!");
                    errors.ForEach(e => Console.WriteLine($"error - [{e.Context}] {e.ErrorMessage}"));
                }
                else
                    Console.WriteLine(result);
            } while (true);
        }

        /// <summary>
        /// Initialize the card manager service
        /// </summary>
        /// <returns></returns>
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