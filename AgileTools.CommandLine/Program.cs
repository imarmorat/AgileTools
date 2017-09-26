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
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace AgileTools.CommandLine
{
    public class Program
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
                    new ShowVariableCommand(),
                    new ConnectToSourceCommand(),
                    new ListSourceCommand()
                };
            context.AvailableCardServices = LoadCardServices("CardServicesConfig.json");

            PrintIntro();

            do
            {
                Console.Write((context.CardService != null ? context.CardService.Id : "(/!\\ no card source selected /!\\)") +  " :: ");
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

        private static IList<CardManagerConfig> LoadCardServices(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException($"File [{filename}] does not exist");

            var content = File.ReadAllText(filename);
            var sources = JsonConvert.DeserializeObject<IList<CardManagerConfig>>(content);
            return sources;
        }

        public static ICardManagerClient CreateSourceFromConfig(CardManagerConfig source)
        {
            var assembly = Assembly.LoadFrom(source.AssemblyName);
            var factory = (ICardManagerFactory) assembly.CreateInstance(source.FactoryClassName);
            if (factory == null)
                throw new Exception($"CardService from assembly [{source.AssemblyName}] and class [{source.FactoryClassName}] is a card service factory (type is {factory.GetType()}");
            return factory.CreateClient();
        }

        public class CardManagerConfig
        {
            public string Id { get; set; }
            public string AssemblyName { get; set; }
            public string FactoryClassName { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        private static void PrintIntro()
        {
            Console.WriteLine("--- Agile Tools ---");
        }
    }
}
