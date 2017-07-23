using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Commands
{
    public class ExitCommand : CommandBase
    {
        public override string CommandName => "exit";
        public override string Description => "Leaves the application";
        public override IEnumerable<CommandParameter> Parameters => new List<CommandParameter>();

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            Console.WriteLine("Bye bye!");
            Environment.Exit(0);
            return null;
        }
    }
}
