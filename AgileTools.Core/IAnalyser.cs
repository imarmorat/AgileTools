using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core
{
    public interface IAnalyser<out TResult>
    {
        string Name { get; }
        TResult Analyse();
    }
}
