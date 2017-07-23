using AgileTools.Analysers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Commands.Modifer
{
    /// <summary>
    /// Allow command output to be exported. 
    /// First parameter is the destination (e.g. file path, email address, etc.),
    /// the second the format of export (e.g. json)
    /// </summary>
    public class ExportCommandModifierHandler : ICommandModifierHandler
    {
        private IList<IResultExporter> _resultExporters;

        public string ModifierKey => "->";

        public ExportCommandModifierHandler()
        {
            _resultExporters = new List<IResultExporter> { new FileExporter() };
        }

        public object Handle(IEnumerable<string> modifier, object cmdOutput)
        {
            var destination = modifier.ElementAt(0);
            var format = modifier.ElementAt(1);

            ExportResult(cmdOutput, destination, format);
            return null;
        }

        /// <summary>
        /// Export the output to the desired destination with specific format.
        /// Format conversion only works if result class implements <see cref="ITransformableResult"/> and 
        /// specifically allows for such transfomation to this format 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="destination"></param>
        /// <param name="format"></param>
        private void ExportResult(object output, string destination, string format)
        {
            var transformedOutput = (object)null;

            // 
            // Do the transformation if possible
            if (output is ITransformableResult transformableOutput)
            {
                if (!transformableOutput.CanTransform(format))
                    throw new Exception($"This result can not be transformed to the given format");

                transformedOutput = transformableOutput.Transform(format);
            }
            else
                transformedOutput = output?.ToString();

            //
            // Find exporter
            var matches = _resultExporters.Where(re => re.CanExportTo(destination));
            if (matches.Count() == 0)
                throw new Exception($"Cannot find any exporter for '{destination}'");

            if (matches.Count() > 1)
                throw new Exception($"Oupsy, more than one exporter found for '{destination}'. Contact me!");

            //
            // Finally do the export
            var exporter = matches.ElementAt(0);
            exporter.Export(transformedOutput, destination);
        }
    }
}
