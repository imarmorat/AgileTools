using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core;
using Newtonsoft.Json;
using System.IO;

namespace AgileTools.Analysers
{
    public class AnalyserHelper
    {
        public static void SaveAnalyserResult(IAnalyser<object> analyser, object result)
        {
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(
                $"{DateTime.Now:yyyy-MMM-dd HHmmsss} - {analyser.Name}.json",
                json);
        }
    }
}
