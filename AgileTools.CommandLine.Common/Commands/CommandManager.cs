﻿using AgileTools.CommandLine.Common.Commands.Modifer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AgileTools.CommandLine.Common.Commands
{
    /// <summary>
    /// Takes care of command execution and notification
    /// </summary>
    public class CommandManager
    {
        #region Private

        private Context _context;
        private IEnumerable<ICommandModifierHandler> _modifierHandlers;
        private const string VariablePattern = @"(?<container>\${(?<varname>[a-zA-Z0-9]+)})";

        #endregion

        public EventHandler<CmdExecutedEventArgs> OnCmdExecuted;

        public IEnumerable<ICommand> KnownCommands { get; set; }

        public CommandManager(Context context)
        {
            _context = context;
            _modifierHandlers = new List<ICommandModifierHandler>
            {
                new ExportCommandModifierHandler()
            };
        }

        public CommandOutput ExecuteCommand(ICommand command, IList<string> parameters)
        {
            //
            // check wether command postfix
            // format is "-> {destination} {format}
            var modifierHandler = (ICommandModifierHandler)null;
            IEnumerable<string> modifierParams = null;
            IEnumerable<string> commandParams = null;

            var modifers =
                from p in parameters
                from mh in _modifierHandlers
                where p == mh.ModifierKey
                select mh;

            if (modifers.Count() > 1)
                return new CommandOutput("Command modifier count is incorrect, expecting 0 or 1.", false);

            if (modifers.Count() == 1)
            {
                modifierHandler = modifers.ElementAt(0);
                var modifierIndex = parameters.IndexOf(modifierHandler.ModifierKey);
                modifierParams = ParseParameter( parameters.Skip(modifierIndex + 1).ToList(),  _context.VariableManager );
                commandParams = parameters.Take(modifierIndex).ToList();
            }
            else
                commandParams = ParseParameter(parameters, _context.VariableManager);

            var output = command.Run(_context, commandParams);

            if (output.IsSuccessful)
                modifierHandler?.Handle(modifierParams, output);
            
            NotifyCmdExecuted(command, parameters);
            return output;
        }

        /// <summary>
        /// Look for variable and change parameter values accordingly
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<string> ParseParameter(IList<string> parameters, VariableManager varManager)
        {
            foreach (var param in parameters)
                yield return ParseParameter(param, varManager);
        }

        /// <summary>
        /// Parse a parameter and replaces any mention of variable with expected value
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ParseParameter(string param, VariableManager varManager)
        {
            var output = param;
            var matches = Regex.Matches(param, VariablePattern);
            foreach(Match match in matches)
            {
                var var = match.Groups["varname"].Value;
                var container = match.Groups["container"].Value;
                var varValue = varManager.Get(var);
                output = output.Replace(container, varValue);
            }

            return output;
        }

        public CommandOutput ExecuteFromString(string commandStr)
        {
            //
            // 1. gather inputs from the string
            var commandQuery = Regex.Matches(commandStr, "[^\\s\"']+|\"([^\"]*)\"|'([^ ']*)\'");
            var commandName = string.Empty;
            var commandParams = new List<string>();
            foreach (Match match in commandQuery)
            {
                if (!match.Success)
                    return new CommandOutput($"Issue parsing argument {match.Index} (value: {match.Value})", false);

                if (match.Index == 0)
                    commandName = match.Value;
                else
                    commandParams.Add(match.Value);
            }
            
            //
            // 2. find associated command
            var command = KnownCommands.FirstOrDefault(c => c.CommandName == commandName);
            if (command == null)
                    return new CommandOutput($"Command '{commandName}' unknown", false);

            //
            // 3. execute command
            var output = ExecuteCommand(command, commandParams);

            return output;
        }

        protected void NotifyCmdExecuted(ICommand cmd, IEnumerable<string> parameters)
        {
            var shadowCopy = OnCmdExecuted;
            if (shadowCopy != null)
                shadowCopy(this, new CmdExecutedEventArgs(cmd, parameters));
        }
    }

    /// <summary>
    /// Used to pass around the invoked command information
    /// </summary>
    public class CmdExecutedEventArgs : EventArgs
    {
        public ICommand Command { get; protected set; }
        public IEnumerable<string> Parameters { get; protected set; }

        public CmdExecutedEventArgs(ICommand cmd, IEnumerable<string> parameters)
        {
            Command = cmd;
            Parameters = parameters;
        }
    }
}
