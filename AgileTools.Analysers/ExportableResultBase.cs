using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public abstract class  ExportableResultBase : ITransformableResult
    {
        protected Dictionary<string, Func<string>> TransformHandlerMapping = new Dictionary<string, Func<string>>();

        public ExportableResultBase()
        {
            TransformHandlerMapping.Add("json", () => ConvertToJson());
            TransformHandlerMapping.Add("txt", () => this.ToString() ));
        }

        public bool CanTransform(string format, string destinationFormat)
        {
            return TransformHandlerMapping.ContainsKey(format) && destinationFormat == "txt";
        }

        public object Transform(string format, string destinationFormat)
        {
            if (!CanTransform(format, destinationFormat))
                throw new Exception($"This output cannot be converted to format {format}");

            return TransformHandlerMapping[format]();
        }

        protected virtual string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
