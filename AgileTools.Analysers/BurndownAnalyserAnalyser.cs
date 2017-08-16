using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class BurndownAnalyserAnalyser : IAnalyser<BurndownResult>
    {
        private IEnumerable<Card> _cards;
        private DateTime _from;
        private DateTime _to;
        private TimeSpan _bucketSize;

        public BurndownAnalyserAnalyser(IEnumerable<Card> cards, DateTime from, DateTime to, TimeSpan bucketSize)
        {
            _cards = cards;
            _from = from;
            _to = to;
            _bucketSize = bucketSize;
        }

        public string Name => "Burndown";

        public BurndownResult Analyse()
        {
            var bucketStartDate = _from.Date;
            var bucketEndDate = (bucketStartDate + _bucketSize).AddSeconds(-1);
            var bdownResult = new BurndownResult();

            while (bucketEndDate <= _to)
            {
                


                bucketStartDate += _bucketSize;
                bucketEndDate += _bucketSize;
            }

            return null;
        }
    }

    public class BurndownResult
    {
        public class Bucket
        {
            public double? Scope { get; set; }
            public double? ScopeAdded { get; set; }
            public double? ScopeRemove { get; set; }
            public double? Completed { get; set; }
        }
    }
}
