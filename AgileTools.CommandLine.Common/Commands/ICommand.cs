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
        object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors);
        string GetUsage(HelpLevel level);
    }
}
