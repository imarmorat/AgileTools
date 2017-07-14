using AgileTools.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AgileTools.Core
{
    public class JiraService
    {
        #region Private 

        private ICardManagerClient _jiraClient;

        #endregion

        #region Public properties

        public IEnumerable<CardStatus> Statuses { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jiraClient"></param>
        public JiraService(ICardManagerClient jiraClient)
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
    }
}
