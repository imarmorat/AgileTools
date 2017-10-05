using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Common.Commands
{
    public class CommandOutput
    {
        private string v1;
        private bool v2;

        /// <summary>
        /// Message that will be displayed to the user
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// list of exception that occured during the command execution
        /// </summary>
        public IList<Exception> Errors { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandOutput()
        {
            Errors = new List<Exception>();
            IsSuccessful = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userMessage"></param>
        /// <param name="exs"></param>
        /// <param name="isSuccessful"></param>
        public CommandOutput(string userMessage, IEnumerable<Exception> exs, bool isSuccessful)
            : this ()
        {
            exs.ForEach(x => Errors.Add(x));
            UserMessage = userMessage;
            IsSuccessful = isSuccessful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userMessage"></param>
        /// <param name="exs"></param>
        /// <param name="isSuccessful"></param>
        public CommandOutput(string userMessage, Exception exs, bool isSuccessful)
            : this(userMessage, new List<Exception> { exs }, isSuccessful)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userMessage"></param>
        /// <param name="isSuccessful"></param>
        public CommandOutput(string userMessage, bool isSuccessful)
            : this()
        {
            UserMessage = userMessage;
            IsSuccessful = isSuccessful;
        }


        /// <summary>
        /// Add an error that occured during command execution.
        /// Command status becomes Fail automatically
        /// </summary>
        /// <param name="ex"></param>
        public void AddError(Exception ex)
        {
            Errors.Add(ex);
            IsSuccessful = false;
        }
    }
}
