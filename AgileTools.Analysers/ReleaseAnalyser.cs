using AgileTools.Core;
using AgileTools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class ReleaseAnalyser : IAnalyser<ReleaseResult>
    {
        #region Private

        private IEnumerable<Card> _cards;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private TimeSpan? _bucketSize;

        #endregion

        public class BucketResult
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            /// <summary>
            /// For each status, gives amount of point in given status in regards to the release timeframe
            /// </summary>
            public Dictionary<CardStatus, double> RollingPointsPerStatus { get; set; }

            /// <summary>
            /// For each status, gives amount of point in given status in regards to the bucket timeframe
            /// </summary>
            public Dictionary<CardStatus, double> PointsPerStatus { get; set; }

            /// <summary>
            /// For each card type, gives the variation in points
            /// </summary>
            public Dictionary<CardType, double> CardTypeVariation { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="bucketSize"></param>
        public ReleaseAnalyser(IEnumerable<Card> cards, DateTime? startDate, DateTime? endDate, TimeSpan? bucketSize)
        {
            _cards = cards; // create new collection?
            _startDate = startDate;
            _endDate = endDate;
            _bucketSize = bucketSize;
        }

        /// <summary>
        /// Perform main analysis
        /// </summary>
        /// <returns></returns>
        public ReleaseResult Analyse()
        {
            var startDate = _startDate ?? _cards.Min(c => c.CreationDate); 
            var endDate = _endDate ?? _cards.Max(c => c.ClosureDate) ?? throw new Exception("Could not determine an end date to use");
            var bucketSize = _bucketSize ?? new TimeSpan(7 * 2, 0, 0, 0);
            var bucketResults = new List<BucketResult>();

            var currDate = startDate;
            do
            {
                bucketResults.Add(AnalyseBucket(currDate, currDate + bucketSize));
            } while (currDate < endDate);


            return new ReleaseResult();
        }

        /// <summary>
        /// Analysis of a particular timewindow
        /// </summary>
        /// <param name="currDate"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private BucketResult AnalyseBucket(DateTime currDate, DateTime dateTime)
        {
            return null;
        }
    }
}
