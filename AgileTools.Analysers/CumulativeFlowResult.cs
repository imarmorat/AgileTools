using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgileTools.Analysers
{
    public class CumulativeFlowResult : ExportableResultBase
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IList<Bucket> Buckets { get; set; }

        public class Bucket
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public IDictionary<CardStatus, double> FlowData { get; set; }
        }

        public CumulativeFlowResult()
        {
            TransformHandlerMapping.Add("csv", ConvertToCsv);
        }

        private string ConvertToCsv()
        {
            if (!Buckets.Any())
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("From,To,");
            Buckets[0].FlowData.Keys.ForEach(s => sb.Append($"\"{s.Name}\","));
            sb.AppendLine();

            Buckets.ForEach(b =>
            {
                sb.Append($"\"{b.From}\",\"{b.To}\",");
                b.FlowData.ForEach(fd => sb.Append($"{fd.Value},"));
                sb.AppendLine();
            });

            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Cumulative Flow from {From:yyyy-MMM-dd} to {To:yyyy-MMM-dd}");
            Buckets.ForEach(b =>
            {
                sb.AppendLine($"\t{b.From:yyyy-MMM-dd} -> {b.To:yyyy-MMM-dd}");
                b.FlowData.ForEach(fd => sb.AppendLine($"\t\t{fd.Key}: {fd.Value}"));
            });
            return sb.ToString();
        }
    }
}