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
        }

        public bool CanTransform(string format)
        {
            return TransformHandlerMapping.ContainsKey(format);
        }

        public object Transform(string format)
        {
            if (!CanTransform(format))
                throw new Exception($"This output cannot be converted to format {format}");

            return TransformHandlerMapping[format]();
        }

        protected virtual string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public interface ITransformableResult
    {
        object Transform(string format);
        bool CanTransform(string format);
    }
}
