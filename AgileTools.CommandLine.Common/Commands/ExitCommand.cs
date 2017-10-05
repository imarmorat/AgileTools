using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.CommandLine.Common.Commands
{
    public class ExitCommand : CommandBase
    {
        public override string CommandName => "exit";
        public override string CommandGroup => "App";
        public override string Description => "Leaves the application";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>();

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            Console.WriteLine("Bye bye!");
            Environment.Exit(0);
            return null;
        }
    }
}
