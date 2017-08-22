using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class BurndownAnalyser : IAnalyser<BurndownResult>
    {
        #region Private members

        private IEnumerable<Card> _cards;
        private DateTime _from;
        private DateTime _to;
        private TimeSpan _bucketSize;
        private double _maxVelocity;
        private double _minVelocity;
        private DateTime _targetDate;

        #endregion

        public string Name => "Burndown";

        public BurndownAnalyser(IEnumerable<Card> cards, DateTime from, DateTime to, DateTime targetDate, TimeSpan? bucketSize, double minVelocity, double maxVelocity)
        {
            _cards = cards;
            _from = from;
            _to = to;
            _targetDate = targetDate;
            _bucketSize = bucketSize ?? new TimeSpan(14, 0, 0, 0);
            _minVelocity = minVelocity;
            _maxVelocity = maxVelocity;
        }

        public BurndownResult Analyse()
        {
            var bucketStartDate = _from.Date;
            var bucketEndDate = (bucketStartDate + _bucketSize).AddSeconds(-1);
            var bdownResult = new BurndownResult();

            while (bucketEndDate <= _to)
            {
                var bucket = new BurndownResult.Bucket() { From = bucketStartDate, To = bucketEndDate };
                bucket.Scope = _cards.Sum(p => p.GetFieldAtDate<double?>(CardFieldMeta.Points, bucketEndDate) ?? 0);
                bucket.NotPointed = _cards.Count(p => 
                    p.CreationDate <= bucket.To && 
                    p.GetFieldAtDate<double?>(CardFieldMeta.Points, bucketEndDate) == null);

                if (bucketStartDate >= DateTime.Now)
                    bucket.Completed = null;
                else
                    bucket.Completed = _cards
                        .Where( p=>
                        {
                            var status = p.GetFieldAtDate<CardStatus>(CardFieldMeta.Status, bucketEndDate);
                            return status != null ?
                                status.Category == StatusCategory.Final :
                                false;
                            })
                        .Sum(p => p.GetFieldAtDate<double?>(CardFieldMeta.Points, bucketEndDate) ?? 0);

                bdownResult.Buckets.Add(bucket);

                bucketStartDate += _bucketSize;
                bucketEndDate += _bucketSize;
            }

            // 
            // main trend line
            var guidelineBuckets = bdownResult.Buckets.Where(b => _targetDate <= b.To);
            var guidelineStep = guidelineBuckets.Last().Scope / guidelineBuckets.Count(); 
            var currGuideline = guidelineStep;
            guidelineBuckets
                .OrderBy(b=> b.From)
                .ForEach(b => { b.Guideline = currGuideline; currGuideline += guidelineStep; });

            // 
            // confidence cone
            var currBucket = bdownResult.Buckets.First(b => b.From <= DateTime.Now && DateTime.Now <= b.To);
            var currConfidenceConeLow = _minVelocity + (currBucket.Completed ?? 0);
            var currConfidenceConeHigh = _maxVelocity + (currBucket.Completed ?? 0);
            bdownResult.Buckets
                .Where(b => DateTime.Now >= b.From)
                .OrderBy(b => b.From)
                .ForEach(b =>
                    {
                        b.ConfidenceConeLow = currConfidenceConeLow;
                        currConfidenceConeLow += _minVelocity;

                        b.ConfidenceConeHigh = currConfidenceConeHigh;
                        currConfidenceConeHigh += _maxVelocity;
                    });

            return bdownResult;
        }
    }
}
