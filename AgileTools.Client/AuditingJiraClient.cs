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
        public AuditingJiraClient(string url, string user, string pwd, IModelConverter modelConverter) :
            base(url, user, pwd, modelConverter)
        {
        }

        public AuditingJiraClient(string url, IAuthenticator authenticator, IModelConverter modelConverter) :
            base(url, authenticator, modelConverter)
        {
        }

        protected override dynamic ExecuteRequest(string resource, Method method, HttpStatusCode expectedCode, bool throwExceptionIfWrongReturnCode = true)
        {
            var response = base.ExecuteRequest(resource, method, expectedCode, false);

            var filename = $"Audit-{DateTime.Now:yyyy-MMM-dd hh-mm-sss-fff}.json";
            var content = new
            {
                Resource = resource,
                HttpMethod = method,
                ExpectedHttpReturnStatus = expectedCode,
                ActualHttpReturnStatus = response.StatusCode,
                When = DateTime.Now,
                ResponseContent = ((IRestResponse)response).Content
            };

            File.WriteAllText(
                Path.Combine(Directory.GetCurrentDirectory(), "/Audit/" + filename),
                JsonConvert.SerializeObject(content)
                );

            if (throwExceptionIfWrongReturnCode)
                CheckReturnCode(response, expectedCode, true);

            return response;
        }
    }
}
