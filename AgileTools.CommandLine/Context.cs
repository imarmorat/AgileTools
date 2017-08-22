using AgileTools.CommandLine.Commands;
using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileTools.CommandLine
{
    public class Context
    {
        public JiraService JiraService { get; set; }

        public IList<Card> LoadedCards { get; set; }

        public CommandManager CmdManager { get; set; }

        public VariableManager VariableManager { get; set; }

        public Context()
        {
            LoadedCards = new List<Card>();
        }
    }
}
