using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AgileTools.Analysers
{
    public class BurndownResult : ExportableResultBase
    {
        public List<Bucket> Buckets { get; set; }

        public class Bucket
        {
            public double? Scope { get; set; }
            public double? Completed { get; set; }
            public double? Guideline { get; set; }
            public DateTime From { get; internal set; }
            public DateTime To { get; internal set; }
            public double? ConfidenceConeLow { get; internal set; }
            public double? ConfidenceConeHigh { get; internal set; }
            public int NotPointed { get; internal set; }
        }

        public BurndownResult()
        {
            Buckets = new List<Bucket>();
            TransformHandlerMapping.Add("csv", ConvertToCsv);
        }

        private string ConvertToCsv()
        {
            if (!Buckets.Any())
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("From,To,Scope,Done,NotPointed,Guideline,ConeLow,ConeHigh");

            Buckets.ForEach(b =>
            {
                sb.AppendLine($"\"{b.From}\",\"{b.To}\",{b.Scope},{b.Completed},{b.Guideline},{b.NotPointed},{b.ConfidenceConeLow},{b.ConfidenceConeHigh}");
            });

            return sb.ToString();
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"BDown result");
            sb.AppendLine($"From\tTo\tScope\tDone\tGuideline\tNotPointed\tConeLow\tConeHigh");
            Buckets.ForEach(b =>
            {
                sb.AppendLine($"{b.From}\t{b.To}\t{b.Scope}\t{b.Completed}\t{b.Guideline}\t{b.NotPointed}\t{b.ConfidenceConeLow}\t{b.ConfidenceConeHigh}");
            });
            return sb.ToString();
        }
    }
}
