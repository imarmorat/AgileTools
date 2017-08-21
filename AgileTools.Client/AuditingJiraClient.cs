using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using AgileTools.Core.Models;

namespace AgileTools.Client
{
    public class AuditingJiraClient : JiraClient
    {
        public AuditingJiraClient(string url, string user, string pwd) :
            base(url, user, pwd)
        {
        }

        public AuditingJiraClient(string url, IAuthenticator authenticator) :
            base(url, authenticator)
        {
            if (!Directory.Exists("Audit"))
                Directory.CreateDirectory("Audit");
        }

        protected override dynamic ExecuteRequest(string resource, Method method, IList<HttpStatusCode> expectedCodes, bool throwExceptionIfWrongReturnCode = true)
        {
            var response = base.ExecuteRequest(resource, method, expectedCodes, false);

            var filename = $"Audit-{DateTime.Now:yyyy-MMM-dd hh-mm-sss-fff}.json";
            var content = new
            {
                Resource = resource,
                HttpMethod = method,
                ExpectedHttpReturnStatus = expectedCodes,
                ActualHttpReturnStatus = response.StatusCode,
                When = DateTime.Now,
                ResponseContent = ((IRestResponse)response).Content
            };

            File.WriteAllText(
                Path.Combine("Audit/" + filename),
                JsonConvert.SerializeObject(content)
                );

            if (throwExceptionIfWrongReturnCode)
                CheckReturnCode(response, expectedCodes, true);

            return response;
        }
    }
}
