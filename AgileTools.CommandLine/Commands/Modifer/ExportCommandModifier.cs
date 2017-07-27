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

        public object Handle(IEnumerable<string> modifierParams, object cmdOutput)
        {
            var destination = modifierParams.ElementAt(0);
            var format = modifierParams.Count() == 2 ? modifierParams.ElementAt(1) : null;

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
            // Find exporter
            var matches = _resultExporters.Where(re => re.CanExportTo(destination));
            if (matches.Count() == 0)
                throw new Exception($"Cannot find any exporter for '{destination}'");

            if (matches.Count() > 1)
                throw new Exception($"Oupsy, more than one exporter found for '{destination}'. Contact me!");

            var exporter = matches.ElementAt(0);

            // 
            // Do the transformation if possible
            if (output is ITransformableResult transformableOutput)
            {
                if (!transformableOutput.CanTransform(format, exporter.ContentFormat))
                    throw new Exception($"This result can not be transformed to the requested format");

                transformedOutput = transformableOutput.Transform(format, exporter.ContentFormat);
            }
            else
            {
                // do a basic conversion if possible
                transformedOutput = output?.ToString();
            }
            
            //
            // Finally do the export
            exporter.Export(transformedOutput, destination);
        }
    }
}
