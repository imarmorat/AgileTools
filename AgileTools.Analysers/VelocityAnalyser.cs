using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class VelocityAnalyser : IAnalyser<VelocityAnalyser.VelocityResult>
    {
        #region Private

        private IEnumerable<Card> _cards;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _bucketSize;

        #endregion

        public string Name { get => "Velocity Analyser";  }
        
        public VelocityAnalyser(IEnumerable<Card> cards, DateTime? startDate, DateTime? endDate, TimeSpan? bucketSize)
        {
            _cards = cards;
            _startDate = startDate ?? cards.Min(c => c.CreationDate);
            _endDate = endDate ?? cards.Max(c => c.ClosureDate) ?? DateTime.Now;
            _bucketSize = bucketSize ?? new TimeSpan(14, 0, 0, 0);
        }

        public VelocityResult Analyse()
        {
            var velResult = new VelocityResult();
            var bucketStartDate = _startDate.Date;
            var bucketEndDate = (bucketStartDate + _bucketSize).AddSeconds(-1);

            while (bucketEndDate <= _endDate)
            {
                velResult.Velocities.Add(
                     (from: bucketStartDate, to: bucketEndDate, velocity: GetPeriodVelocity(_cards, bucketStartDate, bucketEndDate))
                     );

                bucketStartDate += _bucketSize;
                bucketEndDate += _bucketSize;
            }
            //
            // 2. Velocity Histogram
            velResult.Histogram = GetVelocityHistogram(
                velResult.Velocities.Select(v => v.velocity),
                1);

            //
            // 3. Velocity Range of Confidence

            return velResult;
        }

        #region Helpers

        public static double GetPeriodVelocity(IEnumerable<Card> cards, DateTime startDate, DateTime endDate)
        {
            // count as velocity points stories that were in an unfinished state the previous bucket
            // and are marked as successfully implemented at the end of the current bucket.
            // will also be included cards that where marked completed but not successfully.

            var result =
                from card in cards
                let statusBeforeBucket = card.GetFieldAtDate<CardStatus>(CardFieldMeta.Status, startDate.AddSeconds(-1))
                let resolutionBeforeBucket = card.GetFieldAtDate<CardResolution>(CardFieldMeta.Resolution, startDate.AddSeconds(-1))
                let statusInBucket = card.GetFieldAtDate<CardStatus>(CardFieldMeta.Status, endDate)
                let resolutionInBucket = card.GetFieldAtDate<CardResolution>(CardFieldMeta.Resolution, endDate)
                where
                    (statusBeforeBucket == null || (statusBeforeBucket.Category != StatusCategory.Final && resolutionBeforeBucket != CardResolution.CompletedSuccessfully)) &&
                    (statusInBucket != null && statusInBucket.Category == StatusCategory.Final && resolutionInBucket == CardResolution.CompletedSuccessfully)
                select card;

            return result.Sum(c => c.Points) ?? 0;
        }

        public static IEnumerable<(double from, double to, int frequency)> GetVelocityHistogram(IEnumerable<double> velocities, double histogramStep)
        {
            var min = velocities.Min();
            var max = velocities.Max();
            var steps = (max - min) / histogramStep;
            for (var curr = min; curr <= max; curr += histogramStep)
                yield return (from: curr, to: curr + histogramStep, velocities.Count(v => v >= curr && v < curr + histogramStep));
        }

        public static IEnumerable<(double from, double to, int frequency)> GetVelocityHistogram(
            IEnumerable<Card> cards, 
            DateTime startDate, DateTime endDate, TimeSpan bucketSize,
            double histogramStep)
        {
            var bsd = startDate;
            var esd = startDate + bucketSize;
            var velocities = new List<double>();

            while( esd <= endDate )
            {
                velocities.Add( GetPeriodVelocity(cards, bsd, esd) );
                bsd += bucketSize;
                esd += bucketSize;
            }

            return GetVelocityHistogram(velocities, histogramStep);
        }

        #endregion

        public class VelocityResult : ExportableResultBase
        {
            public IList<(DateTime from, DateTime to, double velocity)> Velocities { get; protected set; }
            public IEnumerable<(double from, double to, int frequency)> Histogram { get; internal set; }

            public VelocityResult()
            {
                Velocities = new List<(DateTime from, DateTime to, double velocity)>();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine("Velocities per period:");
                Velocities.ForEach(v => sb.AppendLine($"{v.from} -> {v.to} : {v.velocity}"));

                sb.AppendLine("Histogram:");
                Histogram.ForEach(v => sb.AppendLine($"{v.from} -> {v.to} : {v.frequency}"));

                return sb.ToString();
            }
        }

    }
}
