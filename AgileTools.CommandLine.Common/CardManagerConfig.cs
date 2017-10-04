using System.Collections.Generic;

namespace AgileTools.CommandLine.Common
{
    public class CardManagerConfig
    {
        public string Id { get; set; }
        public string AssemblyName { get; set; }
        public string FactoryClassName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
