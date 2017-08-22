using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine
{
    /// <summary>
    /// Variable are case-sensitive
    /// </summary>
    public class VariableManager
    {
        private Dictionary<string, Func<string>> _variables = new Dictionary<string, Func<string>>();

        /// <summary>
        /// Tells if the variable has been set already
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool IsSet(string varName) => _variables.ContainsKey(varName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        public void Set(string varName, string value)
        {
            if (_variables.ContainsKey(varName))
                _variables[varName] = () => value;
            else
                _variables.Add(varName, () => value);
        }

        /// <summary>
        /// Add the variable. If already exists, replacing existing value with new one provided.
        /// Using this method allows for dynamic value since the lambda expression will be evaluated each time
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="valueFunc"></param>
        public void Set(string varName, Func<string> valueFunc)
        {
            if (_variables.ContainsKey(varName))
                _variables[varName] = valueFunc;
            else
                _variables.Add(varName, valueFunc);
        }

        /// <summary>
        /// Get the variable value
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public string Get(string varName)
        {
            if (!IsSet(varName))
                throw new Exception($"Variable [{varName}] does not exist");

            return _variables[varName]();
        }

        /// <summary>
        /// Returns all the existing variable
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAll()
        {
            var dict = new Dictionary<string, string>();

            foreach (var key in _variables.Keys)
                dict.Add(key, _variables[key]());

            return dict;
        }

        /// <summary>
        /// Removes a variable
        /// </summary>
        /// <param name="varName"></param>
        public void UnSet(string varName)
        {
            if (_variables.ContainsKey(varName))
                _variables.Remove(varName);
        }
    }
}
