﻿using AgileTools.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AgileTools.Core
{
    public class JiraService
    {
        #region Private 

        private IJiraClient _jiraClient;

        #endregion

        #region Public properties

        public IEnumerable<CardStatus> Statuses { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jiraClient"></param>
        public JiraService(IJiraClient jiraClient)
        {
            _jiraClient = jiraClient != null ? jiraClient : throw new ArgumentNullException(nameof(jiraClient));
        }

        /// <summary>
        /// Initialize service. caching data to speed things up
        /// </summary>
        public void Init()
        {
            Statuses = _jiraClient.GetStatuses().ToList();
        }

        /// <summary>
        /// Retrieve tickets from jira database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Card> GetTickets(string query)
        {
            return _jiraClient.GetTickets(query);
        }

        /// <summary>
        /// Get a field value from a card as it was at a specific date
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="card"></param>
        /// <param name="field"></param>
        /// <param name="atDate"></param>
        /// <returns></returns>
        public T GetFieldAtDate<T>(Card card, CardFieldMeta field, DateTime atDate)
        {
            var lastChangesOnFieldPriorDate = card.History
                .Where(h => h.Field == field && h.On <= atDate)
                .OrderByDescending(h => h.On)
                .FirstOrDefault();

            var firstChangesMadeAfterDate = card.History
                .Where(h => h.Field == field && h.On > atDate)
                .OrderBy(h => h.On)
                .FirstOrDefault();

            if (lastChangesOnFieldPriorDate != null)
                return lastChangesOnFieldPriorDate.To.ChangeTo<T>();

            if (firstChangesMadeAfterDate != null)
                return firstChangesMadeAfterDate.From.ChangeTo<T>();

            return card[field].ChangeTo<T>();
        }
    }
}