using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Common.Commands
{
    public class SetVariableCommand : CommandBase
    {
        public override string CommandName => "setvar";
        public override string CommandGroup => "Variable Management";
        public override string Description => "set a value to a particular variable";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>()
            {
                new CommandParameter.StringParameter("varname", "variable name; if it doesnt exist already, it will be added. Otherwise value is updated", false),
                new CommandParameter.StringParameter("value", "variable value", false),
            };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var varName = (string) ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var varValue = (string) ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1));

            context.VariableManager.Set(varName, varValue);
            return new CommandOutput("Variable set", true);
        }
    }

    public class UnSetVariableCommand : CommandBase
    {
        public override string CommandName => "unsetvar";
        public override string CommandGroup => "Variable Management";
        public override string Description => "remove a particular variable";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>()
            {
                new CommandParameter.StringParameter("varname", "variable name; if it doesnt exist already, it will be added. Otherwise value is updated", false),
            };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            var varName = (string)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));

            if (!context.VariableManager.IsSet(varName))
                return new CommandOutput("Variable is not set", false);
            else
            {
                context.VariableManager.UnSet(varName);
                return new CommandOutput("Variable unset", true);
            }
        }
    }

    public class ShowVariableCommand : CommandBase
    {
        public override string CommandName => "showvar";
        public override string CommandGroup => "Variable Management";
        public override string Description => "show one or all variables already registereds";
        public override IEnumerable<CommandParameter> ExpectedParameters => new List<CommandParameter>()
            {
                new CommandParameter.StringParameter("varname", "Leave empty if you want to show all vars", false),
            };

        public override CommandOutput Run(Context context, IEnumerable<string> parameters)
        {
            if (parameters.Count() == 0)
            {
                var vars = context.VariableManager.GetAll();
                var sb = new StringBuilder();
                sb.AppendLine("| var \t\t | value \t\t |");
                sb.AppendLine("------------------------------------------");
                foreach (var name in vars.Keys)
                    sb.AppendLine($"| {name} \t\t | {vars[name]} \t\t |");
                sb.AppendLine("------------------------------------------");
                return new CommandOutput(sb.ToString(), true);
            }
            else
            {
                var varName = (string)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
                return new CommandOutput(context.VariableManager.Get(varName), true);
            }
        }
    }

}
