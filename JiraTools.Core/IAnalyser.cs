using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTools.Core
{
    public interface IAnalyser<TResult>
    {
        TResult Analyse();
    }
}
