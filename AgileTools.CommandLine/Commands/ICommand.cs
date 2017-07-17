using System;
using System.Collections.Generic;
using System.Text;

namespace AgileTools.CommandLine.Commands
{
    public interface ICommand
    {
        string CommandName { get; }
        bool TryParse(IEnumerable<string> parameters, out IList<ParameterError> paramErrors);
        string Run(Context context, IEnumerable<string> parameters);
        string GetUsage();
    }

    public class ParameterError
    {
        public string ParameterName { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public ParameterError(string paramName, string errorMessage)
        {
            ParameterName = !string.IsNullOrEmpty(paramName) ? paramName : throw new ArgumentNullException(nameof(paramName));
            ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : throw new ArgumentNullException(nameof(errorMessage));
        }
    }
}
