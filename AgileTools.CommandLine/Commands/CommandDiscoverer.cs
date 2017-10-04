using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
{
    public class CommandDiscoverer
    {
        private static ILog _logger = LogManager.GetLogger(typeof(CommandDiscoverer));

        public static IEnumerable<ICommand> Discover(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.dll").Union( Directory.EnumerateFiles(path, "*.exe"));
            return files.SelectMany(f => DiscoverFromFile(f));
        }

        private static IEnumerable<ICommand> DiscoverFromFile(string filename)
        {
            _logger.Debug($"Analysing file {filename}");

            var assembly = Assembly.LoadFile(filename);
            return assembly
                .GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .Select(t =>
                {
                    var instance = (ICommand)Activator.CreateInstance(t);
                    _logger.Debug($"Command {t.Name} instanced from {assembly.FullName}");
                    return instance;
                }).ToList();
        }
    }
}
