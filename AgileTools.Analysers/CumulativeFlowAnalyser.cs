﻿using AgileTools.Core;
using AgileTools.Core.Models;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Analysers
{
    public class CumulativeFlowAnalyser : IAnalyser<CumulativeFlowResult>
    {
        #region Private

        private static readonly ILog _logger = LogManager.GetLogger(typeof(CumulativeFlowAnalyser));
        private IEnumerable<Card> _cards;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private TimeSpan _bucketSize;
        private JiraService _jiraService; 
        
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jiraService">jiraService used to get the related tickets</param>
        /// <param name="tickets">list of tickets that will be analysed</param>
        /// <param name="bucketSize">size of the bucket. if not specifed, set to a day</param>
        /// <param name="from">start date - if not specified, taking the earliest created ticket</param>
        /// <param name="to">end date - if not specified, taking the latest ticket closure</param>
        public CumulativeFlowAnalyser(JiraService jiraService, IEnumerable<Card> tickets, TimeSpan? bucketSize = null,DateTime? from = null, DateTime? to = null)
        {
            _jiraService = jiraService ?? throw new ArgumentNullException(nameof(jiraService));
            _cards = tickets;
            _dateFrom = from ?? _cards.Min(t => t.CreationDate);
            _dateTo = to ?? _cards.Max(t => t.ClosureDate) ?? throw new ArgumentException("Cannot infer an end date", nameof(to));
            _bucketSize = bucketSize ?? new TimeSpan(1, 0, 0);

            if (_dateFrom > _dateTo)
                throw new ArgumentException("Date from is after Date to");
        }

        public CumulativeFlowResult Analyse()
        {
            _logger.Info($"Starting cumulative flow analysis on period from {_dateFrom.Date} to {_dateTo.Date}");

            var output = new CumulativeFlowResult() { From = _dateFrom.Date, To = _dateTo.Date, Buckets = new List<CumulativeFlowResult.Bucket>() };
            var currDate = _dateFrom.Date;
            while(currDate <= _dateTo.Date)
            {
                var bucket = new CumulativeFlowResult.Bucket
                {
                    From = currDate,
                    To = currDate + _bucketSize,
                    FlowData = new Dictionary<CardStatus, double>()
                };

                _jiraService.Statuses.ForEach(s =>
                {
                    var totalPoints = _cards
                        .Where(card => card.GetFieldAtDate<CardStatus>(CardFieldMeta.Status, bucket.To) == s)
                        .Sum( card => card.GetFieldAtDate<double>(CardFieldMeta.Points, bucket.To));
                    bucket.FlowData.Add(s, totalPoints);
                });

                output.Buckets.Add(bucket);
                currDate += _bucketSize;
            }

            _logger.Info($"Cumulative flow analysis completed");

            return output;
        }
    }
}
