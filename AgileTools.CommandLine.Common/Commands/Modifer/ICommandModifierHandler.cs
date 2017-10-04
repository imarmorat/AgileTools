using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Common.Commands.Modifer
{
    public interface ICommandModifierHandler
    {
        /// <summary>
        /// In the command line, key that will when to parse modifier parameters
        /// </summary>
        string ModifierKey { get; }

        /// <summary>
        /// Apply modifier on the command ouput
        /// </summary>
        /// <param name="modifierParameters"></param>
        /// <param name="cmdOutput"></param>
        /// <returns></returns>
        object Handle(IEnumerable<string> modifierParameters, object cmdOutput);
    }
}
