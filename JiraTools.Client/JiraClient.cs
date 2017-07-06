using JiraTools.Core;
using System;
using JiraTools.Core.Models;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using Newtonsoft.Json;

namespace JiraTools.Client
{
    /// <summary>
    /// TODO: cache the fields as for now using GetFields()
    /// </summary>
    public class JiraClient : IJiraClient
    {
        private IRestClient _restClient;
        private string RestPrefix = "/rest/api/latest";
        private IModelConverter _modelConverter;

        /// <summary>
        /// Constructor
        /// </summary>
        public JiraClient(string url, string user, string pwd, IModelConverter modelConverter) :
            this(url, new HttpBasicAuthenticator(user,pwd), modelConverter)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authenticator"></param>
        public JiraClient(string url, IAuthenticator authenticator, IModelConverter modelConverter)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            _modelConverter = modelConverter ?? throw new ArgumentNullException(nameof(modelConverter));

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

        /// <summary>
        /// Get all fields
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JiraField> GetFields()
        {
            var response = ExecuteRequest($"{RestPrefix}/field", Method.GET, HttpStatusCode.OK);
            foreach (var field in response.Data)
                yield return _modelConverter.ConvertField(field);
        }

        public IEnumerable<CardStatus> GetStatuses()
        {
            var response = ExecuteRequest($"{RestPrefix}/status", Method.GET, HttpStatusCode.OK);
            foreach (var status in response.Data)
                yield return _modelConverter.ConvertStatus( status);
        }

        public Card GetTicket(string ticketId)
        {
            var response = ExecuteRequest(
                $"{RestPrefix}/issue/{ticketId}&expand=changelog&fields=*all,comment",
                Method.GET,
                HttpStatusCode.OK);
            return _modelConverter.ConvertTicket(response.Data, GetFields());
        }

        public IEnumerable<Card> GetTickets(string query)
        {
            var index = 0;
            var total = 0;

            do
            {
                var response = ExecuteRequest(
                    $"{RestPrefix}/search?jql={query}&expand=changelog&fields=*all,comment&startAt={index}",
                    Method.GET,
                    HttpStatusCode.OK);

                index += (int)response.Data.maxResults;
                total = (int)response.Data.total;

                foreach (var ticket in response.Data.issues)
                    yield return _modelConverter.ConvertTicket(ticket, GetFields()); //ticket;
            } while (index < total);
        }

        protected virtual dynamic ExecuteRequest(string resource, Method method,  HttpStatusCode expectedCode, bool throwExceptionIfWrongReturnCode = true)
        {
            var request = new RestRequest(resource, method) { RequestFormat = DataFormat.Json };
            var response = _restClient.Execute<dynamic>(request);

            CheckReturnCode(response, expectedCode, throwExceptionIfWrongReturnCode);

            return response;
        }

        protected static void CheckReturnCode(IRestResponse<dynamic> response, HttpStatusCode expectedCode, bool throwExceptionIfWrongReturnCode)
        {
            if (throwExceptionIfWrongReturnCode && response.StatusCode != expectedCode)
                throw new Exception($"REST response returned an unexpected code (is {response.StatusCode}, expecting {expectedCode}");
        }
    }
}
