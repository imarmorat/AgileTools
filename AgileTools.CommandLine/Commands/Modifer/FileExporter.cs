using System.IO;
using System.Text.RegularExpressions;

namespace AgileTools.CommandLine.Commands
{
    /// <summary>
    /// Export result to file
    /// </summary>
    public class FileExporter : IResultExporter
    {
        /// <summary>
        /// Can export if it looks like a path + filename or just a filename
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool CanExportTo(string destination)
        {
            // http://regexr.com/
            //return Regex.IsMatch(destination, @"([\w\:\\\w\-_.()~!@#$%^&=+';,{} /]+\w+\.[\w_\-.()~!@#$%^&=+';,{}]+)");
            return Regex.IsMatch(destination, @"([\w\:\\\w\-_.()~!@#$%^&=+';,{} /]+\.[\w_\-.()~!@#$%^&=+';,{}]+)");
        }

        /// <summary>
        /// Export to file, overwrites if it exists already
        /// </summary>
        /// <param name="result"></param>
        /// <param name="destination"></param>
        public void Export(object result, string destination)
        {
            File.WriteAllText(destination, result?.ToString());
        }
    }
}
