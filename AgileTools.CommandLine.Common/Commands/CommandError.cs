using System;

namespace AgileTools.CommandLine.Common.Commands
{
    public class CommandError
    {
        public string Context { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public CommandError(string context, string errorMessage)
        {
            Context = !string.IsNullOrEmpty(context) ? context : throw new ArgumentNullException(nameof(context));
            ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : throw new ArgumentNullException(nameof(errorMessage));
        }
    }
}
