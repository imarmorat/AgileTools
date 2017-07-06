using JiraTools.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JiraTools.Core
{
    public class JiraService
    {
        #region Private 

        private IJiraClient _jiraClient;

        #endregion

        #region Public properties

        public IEnumerable<CardStatus> Statuses { get; protected set; }

        //public IEnumerable<JiraField> Fields { get; protected set; }

        //public IEnumerable<JiraField> CommonFields { get; protected set; }

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
            //Fields = _jiraClient.GetFields().ToList();
        }

        ///// <summary>
        ///// Get a field based on its name. Case insensitive
        ///// </summary>
        ///// <param name="fieldName"></param>
        ///// <returns></returns>
        //public JiraField GetFieldByName(string fieldName)
        //{
        //    var match = Fields.Where(field => string.Compare(field.Name, fieldName, true) == 0).FirstOrDefault();
        //    return match;
        //}

        ///// <summary>
        ///// Get a field based on its name. Case insensitive
        ///// </summary>
        ///// <param name="fieldName"></param>
        ///// <returns></returns>
        //public JiraField GetFieldById(string fieldId)
        //{
        //    var match = Fields.Where(field => string.Compare(field.Id, fieldId, true) == 0).FirstOrDefault();
        //    return match;
        //}

        /// <summary>
        /// Retrieve tickets from jira database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Card> GetTickets(string query)
        {
            var totalTickets = 0;
            var totalLoaded = 0;

            return _jiraClient.GetTickets(query);
        }

        public bool GetFieldAtDate<T>(Card t, CardStatus s, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
