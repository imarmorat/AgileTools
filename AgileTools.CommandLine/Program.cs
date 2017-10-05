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
using AgileTools.CommandLine.Common;
using AgileTools.CommandLine.Common.Commands;

namespace AgileTools.CommandLine
{
    public partial class Program
    {
        private static ILog _logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var context = new Context();
            context.VariableManager = new VariableManager();
            context.VariableManager.Set("now.date", () => DateTime.Now.Date.ToString("dd-MMM-yyyy"));
            context.VariableManager.Set("now.time", () => DateTime.Now.ToString("HH:mm"));
            context.CmdManager = new CommandManager(context);
            var commands = CommandDiscoverer.Discover(Directory.GetCurrentDirectory());
            commands.ForEach(c => c.CommandManager = context.CmdManager);
            context.CmdManager.KnownCommands = new List<ICommand>(commands);
            context.AvailableCardServices = LoadCardServices("CardServicesConfig.json");

            PrintIntro();

            do
            {
                Console.Write((context.CardService != null ? context.CardService.Id : "(/!\\ no card source set /!\\)") +  " :: ");
                var cmdLine = Console.ReadLine();

                try
                {
                    var result = context.CmdManager.ExecuteFromString(cmdLine);

                    if (!result.IsSuccessful)
                    {
                        Console.WriteLine($"Command failed: {result.UserMessage}");
                        result.Errors.ForEach(e => Console.WriteLine($"error - {e.Message}"));
                    }
                    else
                        Console.WriteLine(result.UserMessage);
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



        private static void PrintIntro()
        {
            Console.WriteLine("--- Agile Tools ---");
        }
    }
}
