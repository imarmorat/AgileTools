using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ExitCommand : ICommand
    {
        public string CommandName => "exit";
        public string Description => "Leaves the application";
        public IEnumerable<CommandParameter> Parameters => new List<CommandParameter>();

        public string Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            Console.WriteLine("Bye bye!");
            Environment.Exit(0);
            return null;
        }
    }
}
