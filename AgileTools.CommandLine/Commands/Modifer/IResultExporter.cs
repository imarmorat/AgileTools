namespace AgileTools.CommandLine.Commands
{
    /// <summary>
    /// Defines a class that can export result generated out of the commands to a specific destination
    /// </summary>
    public interface IResultExporter
    {
        bool CanExportTo(string destination);
        void Export(object result, string destination);
    }
}
