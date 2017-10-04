using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands
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

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var varName = (string) ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
            var varValue = (string) ExpectedParameters.ElementAt(1).Convert(parameters.ElementAt(1));

            context.VariableManager.Set(varName, varValue);
            return "Variable set";
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

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
        {
            var varName = (string)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));

            if (!context.VariableManager.IsSet(varName))
            {
                errors.Add(new CommandError("Variable Manager", "Variable is not set"));
                return "Variable unset failed";
            }
            else
            {
                context.VariableManager.UnSet(varName);
                return "Variable unset";
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

        public override object Run(Context context, IEnumerable<string> parameters, ref IList<CommandError> errors)
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
                return sb.ToString();
            }
            else
            {
                var varName = (string)ExpectedParameters.ElementAt(0).Convert(parameters.ElementAt(0));
                return context.VariableManager.Get(varName);
            }
        }
    }

}
