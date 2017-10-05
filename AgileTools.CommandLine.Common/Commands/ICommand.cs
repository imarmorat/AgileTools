using System.Collections.Generic;
using System.Text;

namespace AgileTools.CommandLine.Common.Commands
{
    public interface ICommand
    {
        CommandManager CommandManager { get; set; }
        string CommandName { get; }
        string CommandGroup { get; }
        string Description { get; }
        IEnumerable<CommandParameter> ExpectedParameters { get; }
        CommandOutput Run(Context context, IEnumerable<string> parameters);
        string GetUsage(HelpLevel level);
    }
}
