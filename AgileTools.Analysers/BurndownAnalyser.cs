using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class BurndownAnalyser : IAnalyser<BurndownResult>
    {
        private IEnumerable<Card> _cards;
        private DateTime _from;
        private DateTime _to;
        private TimeSpan _bucketSize;

        public string Name => "Burndown";

        public BurndownAnalyser(IEnumerable<Card> cards, DateTime from, DateTime to, TimeSpan? bucketSize)
        {
            _cards = cards;
            _from = from;
            _to = to;
            _bucketSize = bucketSize ?? new TimeSpan(14, 0, 0, 0);
        }

        public BurndownResult Analyse()
        {
            var bucketStartDate = _from.Date;
            var bucketEndDate = (bucketStartDate + _bucketSize).AddSeconds(-1);
            var bdownResult = new BurndownResult();

            while (bucketEndDate <= _to)
            {
                var bucket = new BurndownResult.Bucket();
                bucket.Scope = _cards.Sum(p => p.GetFieldAtDate<double?>(CardFieldMeta.Points, bucketEndDate) ?? 0);
                bucket.Completed = _cards
                    .Where( p=> p.GetFieldAtDate<CardStatus>(CardFieldMeta.Status, bucketEndDate).Category == StatusCategory.Final )
                    .Sum(p => p.GetFieldAtDate<double?>(CardFieldMeta.Points, bucketEndDate) ?? 0);

                bdownResult.Buckets.Add(bucket);

                bucketStartDate += _bucketSize;
                bucketEndDate += _bucketSize;
            }

            return bdownResult;
        }
    }

    public class BurndownResult
    {
        public List<Bucket> Buckets { get; set; }

        public class Bucket
        {
            public double? Scope { get; set; }
            public double? Completed { get; set; }
            public double? Guideline { get; set; }
        }

        public BurndownResult()
        {
            Buckets = new List<Bucket>();
        }
    }
}
