using System;
using RestSharp;
using RestSharp.Deserializers;
using Newtonsoft.Json;

namespace AgileTools.Client
{
    internal class JsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }
}