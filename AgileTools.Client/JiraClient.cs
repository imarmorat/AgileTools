﻿using AgileTools.Core;
using System;
using AgileTools.Core.Models;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using Newtonsoft.Json;
using log4net;
using System.Diagnostics;

namespace AgileTools.Client
{
    /// <summary>
    /// TODO: cache the fields as for now using GetFields()
    /// </summary>
    public class JiraClient : ICardManagerClient
    {
        #region Private

        private static ILog _logger = LogManager.GetLogger(typeof(JiraClient));
        private IRestClient _restClient;
        private string MainRestPrefix = "/rest/api/latest";
        private string AgileRestPrefix = "/rest/agile/latest";
        private IModelConverter _modelConverter;
        private IList<HttpStatusCode> _allowedResponseStatusCode = new List<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotFound };
        
        #endregion

        public IModelConverter ModelConverter {
            get => _modelConverter ?? throw new Exception("Model Converter not set");
            set => _modelConverter = value ?? throw new ArgumentNullException("ModelConverter");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public JiraClient(string url, string user, string pwd) :
            this(url, new HttpBasicAuthenticator(user,pwd))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authenticator"></param>
        public JiraClient(string url, IAuthenticator authenticator)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            _restClient = new RestClient(url)
            {
                Authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator)),
            };

            _restClient.AddHandler("application/json", new JsonDeserializer());
        }

        /// <summary>
        /// Comment a ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="comment"></param>
        /// <param name="author"></param>
        public void CommentTicket(string ticketId, string comment, string author = null)
        {
            throw new NotImplementedException();
        }

        public Sprint GetSprint(string sprintId)
        {
            var response = ExecuteRequest($"{AgileRestPrefix}/sprint/{sprintId}", Method.GET);
            return response.StatusCode == HttpStatusCode.OK ?
                ModelConverter.ConvertSprint(response.Data) :
                null;
        }

        public CardStatus GetStatus(string statusId)
        {
            var response = ExecuteRequest($"{MainRestPrefix}/status/{statusId}", Method.GET);
            return response.StatusCode == HttpStatusCode.OK ?
                ModelConverter.ConvertStatus(response.Data) :
                null;
        }

        public User GetUser(string userId)
        {
            var response = ExecuteRequest($"{MainRestPrefix}/user?key={userId}", Method.GET);
            return response.StatusCode == HttpStatusCode.OK ?
                ModelConverter.ConvertUser(response.Data) :
                null;
        }

        /// <summary>
        /// Get all fields
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JiraField> GetFields()
        {
            var list = new List<JiraField>();

            var response = ExecuteRequest($"{MainRestPrefix}/field", Method.GET);
            if (response.StatusCode != HttpStatusCode.OK)
                return list;

            foreach (var field in response.Data)
                list.Add(ModelConverter.ConvertField(field));

            return list;
        }

        public IEnumerable<CardStatus> GetStatuses()
        {
            var list = new List<CardStatus>();

            var response = ExecuteRequest($"{MainRestPrefix}/status", Method.GET);
            if (response.StatusCode != HttpStatusCode.OK)
                return list;

            foreach (var status in response.Data)
                list.Add(ModelConverter.ConvertStatus(status));

            return list;
        }

        public Card GetTicket(string ticketId)
        {
            var response = ExecuteRequest(
                $"{MainRestPrefix}/issue/{ticketId}&expand=changelog&fields=*all,comment",
                Method.GET);

            return response.StatusCode == HttpStatusCode.OK ? 
                ModelConverter.ConvertCard(response.Data, GetFields()) : 
                null;
        }

        public IEnumerable<Card> GetTickets(string query)
        {
            var index = 0;
            var total = 0;
            var fields = GetFields();
            var cards = new List<Card>();
            var stopWatch = new Stopwatch();

            do
            {
                _logger.Debug($"Executing card fectch query...");
                stopWatch.Reset();
                stopWatch.Start();

                var response = ExecuteRequest(
                    $"{MainRestPrefix}/search?jql={query}&expand=changelog&fields=*all,comment&startAt={index}",
                    Method.GET);

                index += (int)response.Data.maxResults;
                total = (int)response.Data.total;

                foreach (var ticket in response.Data.issues)
                     cards.Add( ModelConverter.ConvertCard(ticket, fields) ); //ticket;

                stopWatch.Stop();
                _logger.Debug($"Fetched {total} so far; last bacth fetch in {stopWatch.ElapsedMilliseconds}ms");

            } while (index < total);

            _logger.Info($"Query fetched {cards.Count} cards");

            return cards;
        }

        #region Protected methods

        protected virtual dynamic ExecuteRequest(string resource, Method method, bool throwExceptionIfWrongReturnCode = true)
        {
            return ExecuteRequest(resource, method, _allowedResponseStatusCode, throwExceptionIfWrongReturnCode);
        }

        protected virtual dynamic ExecuteRequest(string resource, Method method,  IList<HttpStatusCode> expectedCode, bool throwExceptionIfWrongReturnCode = true)
        {
            var request = new RestRequest(resource, method) { RequestFormat = DataFormat.Json };
            var response = _restClient.Execute<dynamic>(request);

            CheckReturnCode(response, expectedCode, throwExceptionIfWrongReturnCode);

            return response;
        }

        protected static void CheckReturnCode(IRestResponse<dynamic> response, IList<HttpStatusCode> expectedCode, bool throwExceptionIfWrongReturnCode)
        {
            if (throwExceptionIfWrongReturnCode && !expectedCode.Contains(response.StatusCode))
                throw new Exception($"REST response returned an unexpected code (is {response.StatusCode}, expecting {expectedCode}).");
        }

        #endregion
    }
}
