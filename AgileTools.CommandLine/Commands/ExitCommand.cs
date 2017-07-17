using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ExitCommand : ICommand
    {
        public string CommandName => "exit";

        public string GetUsage()
        {
            return $"{CommandName} - leaves the application";
        }

        public string Run(Context context, IEnumerable<string> parameters)
        {
            Console.WriteLine("Bye bye!");
            Environment.Exit(0);
            return null;
        }

        public bool TryParse(IEnumerable<string> parameters, out IList<ParameterError> paramErrors)
        {
            paramErrors = new List<ParameterError>();
            return true;
        }
    }
}
