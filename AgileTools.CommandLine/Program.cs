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
using log4net;

namespace AgileTools.CommandLine
{
    class Program
    {
        private static ILog _logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var context = new Context();
            context.VariableManager = new VariableManager();
            context.VariableManager.Set("now.date", () => DateTime.Now.Date.ToString("dd-MMM-yyyy"));
            context.VariableManager.Set("now.time", () => DateTime.Now.ToString("HH:mm"));
            context.CmdManager = new CommandManager(context);
            context.CmdManager.KnownCommands = new List<ICommand>
                {
                    new GetCommandHelpCommand(),
                    new FetchCardsCommand(),
                    new ListCardsCommand(),
                    new ExitCommand(),
                    new RunAnalyserCommand(),
                    new MacroCommand(context.CmdManager), // I dont like my design, looks dodgy. change that later. context is passed around so should use it to access cmdmanager
                    new LoadCardsCommand(),
                    new SaveCardsCommand(),
                    new SetVariableCommand(),
                    new UnSetVariableCommand(),
                    new ShowVariableCommand()
                };

            PrintIntro();
            context.CardService = InitCardManagerService(args.Length == 1 ? args[0] : null);

            do
            {
                Console.Write(":: ");
                var cmdLine = Console.ReadLine();

                try
                {
                    var errors = (IList<CommandError>)new List<CommandError>();
                    var result = context.CmdManager.ExecuteFromString(cmdLine, ref errors);

                    if (errors.Any())
                    {
                        Console.WriteLine("Command failed!");
                        errors.ForEach(e => Console.WriteLine($"error - [{e.Context}] {e.ErrorMessage}"));
                    }
                    else
                        Console.WriteLine(result);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Command crashed! {ex.Message} - {ex.GetType()}");
                    _logger.Warn("Command crashed", ex);
                }
            } while (true);
        }

        /// <summary>
        /// Initialize the card manager service
        /// </summary>
        /// <returns></returns>
        private static ICardManagerClient InitCardManagerService(string cardClientUrl)
        {
            Console.Write("user: ");
            var userName = Console.ReadLine();
            Console.Write("pwd: ");
            var pwd = Utils.ReadPasswordFromConsole();

            var cardClient = (ICardManagerClient)new AuditingJiraClient(
                cardClientUrl ?? "http://10.0.75.1:8080", 
                userName, 
                pwd);

            cardClient = new CachedJiraClient(cardClient);
            cardClient.ModelConverter = new DefaultModelConverter(cardClient);

            var user = cardClient.GetUser(userName);
            Console.WriteLine($"\r\nWelcome {user.FullName} ({user.Id} / {user.Email})");

            return cardClient;
        }

        private static void PrintIntro()
        {
            Console.WriteLine("--- Agile Tools ---");
        }
    }
}
