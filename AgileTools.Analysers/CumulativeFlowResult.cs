using AgileTools.Core.Models;
using System;
using System.Collections.Generic;

namespace AgileTools.Analysers
{
    public class CumulativeFlowResult
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
    }
}