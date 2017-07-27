namespace AgileTools.CommandLine.Commands
{
    /// <summary>
    /// Defines a class that can export result generated out of the commands to a specific destination
    /// </summary>
    public interface IResultExporter
    {
        /// <summary>
        /// Format that is required by the exporter
        /// </summary>
        string ContentFormat { get; }

        /// <summary>
        /// Ensures that the exporter can export to the specific destination
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        bool CanExportTo(string destination);

        void Export(object result, string destination);
    }
}
