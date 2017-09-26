using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileTools.Core.Models;

namespace AgileTools.Client
{
    public class JiraCardManagerFactory : ICardManagerFactory
    {
        /// <summary>
        /// Creates a default client. If needed, can overload the method with a different model converter
        /// </summary>
        /// <returns></returns>
        public ICardManagerClient CreateClient()
        {
            var client = new CachedJiraClient( new JiraClient() );
            client.ModelConverter = new DefaultModelConverter(client);
            return client;
        }
    }
}
