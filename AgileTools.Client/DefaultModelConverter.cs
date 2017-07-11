using AgileTools.Core;
using AgileTools.Core.Models;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AgileTools.Core.Models.Card;

namespace AgileTools.Client
{
    /// <summary>
    /// This model converter is tested for JIRA 7.3.0 and below (havent tested below 7.0)
    /// </summary>
    public class DefaultModelConverter : IModelConverter
    {
        #region Protected

        protected static ILog _logger = LogManager.GetLogger(typeof(DefaultModelConverter));
        protected List<Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>> _jiraFieldMapping;
        protected Dictionary<string, StatusCategory> _jiraStatusCategoryMapping = new Dictionary<string, StatusCategory>
        {
            { "new", StatusCategory.New },
            { "indeterminate", StatusCategory.InProgress },
            { "done", StatusCategory.Final }
        };
        private Dictionary<string, CardType> _jiraTicketTypeMapping = new Dictionary<string, CardType>
        {
            { "Story", CardType.Story },
            { "Bug", CardType.Bug },
            { "Epic", CardType.Feature },
            { "Task", CardType.Enabler }
        };

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultModelConverter()
        {
            _jiraFieldMapping = new List<Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>>
         {
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Summary",
                    CardFieldMeta.Title,
                    o=> (string) o,
                    (str1, str2,client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Issue Type",
                    CardFieldMeta.Type,
                    o=> IdentifyCardType((string) o.name),
                    (str1, str2, client) => IdentifyCardType(str2)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Description",
                    CardFieldMeta.Description,
                    o=> (string) o,
                    (str1, str2, client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Created",
                    CardFieldMeta.Created,
                    o=> (DateTime) o,
                    (str1, str2, client) => DateTime.Parse(str2)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Resolved",
                    CardFieldMeta.ResolutionDate,
                    o=> (DateTime?) o,
                    (str1, str2, client) => string.IsNullOrEmpty(str2) ? (DateTime?) null : DateTime.Parse(str2)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Due Date",
                    CardFieldMeta.DueDate,
                    o=> (DateTime?) o,
                    (str1, str2, client) => string.IsNullOrEmpty(str2) ? (DateTime?) null : DateTime.Parse(str2)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Story Points",
                    CardFieldMeta.Points,
                    o=> (int?) o,
                    (str1, str2, client) => string.IsNullOrEmpty(str2) ? (double?) null : Double.Parse(str2)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Sprint",
                    CardFieldMeta.Sprint,
                    o=> ExtractSprintDetailsFromString(o),
                    (str1, str2, client) => string.IsNullOrEmpty(str1) ? (Sprint) null : (Sprint) client.GetSprint((string)str1) // str1 = sprintId, str2 = sprintNames
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Flagged",
                    CardFieldMeta.Flagged,
                    o=> o != null,
                    (str1, str2, client) => str2 == "Impediment" // throw new NotImplementedException()
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Resolution",
                    CardFieldMeta.Resolution,
                    o=> o != null ? (string) o.name : null,
                    (str1, str2, client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Status",
                    CardFieldMeta.Status,
                    o=> ConvertStatus(o),
                    (str1, str2, client) =>  string.IsNullOrEmpty(str1) ? (CardStatus) null : client.GetStatus( (string)str1 )// str1 = statusId, str2 = statusname
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func< string, string, IJiraClient, object>>(
                    "Epic Link",
                    CardFieldMeta.EpicId,
                    o=> (string) o,
                    (str1, str2, client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Rank",
                    CardFieldMeta.Rank,
                    o=> (string) o,
                    (str1, str2, client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Labels",
                    CardFieldMeta.Labels,
                    o=> ExtractList<string>(o),
                    (str1, str2, client) => str2
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Assignee",
                    CardFieldMeta.Assignee,
                    o=> ConvertUser(o),
                    (str1, str2, client) => string.IsNullOrEmpty(str1) ? (User) null : client.GetUser((string) str1)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Reporter",
                    CardFieldMeta.Reporter,
                    o=> ConvertUser(o),
                    (str1, str2, client) => string.IsNullOrEmpty(str1) ? (User) null : client.GetUser((string) str1)
                    ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>, Func<string, string, IJiraClient, object>>(
                    "Creator",
                    CardFieldMeta.Creator,
                    o=> ConvertUser(o),
                    (str1, str2, client) => string.IsNullOrEmpty(str1) ? (User) null : client.GetUser((string) str1)
                    ),
            };
        }

        public Sprint ConvertSprint(dynamic sprint)
        {
            return new Sprint
            {
                Id = (string)sprint.id,
                Name = (string)sprint.name,
                StartDate = (DateTime?)sprint.startDate,
                EndDate = (DateTime?)sprint.endDate,
                BoardId = (string)sprint.originBoardId
            };
        }

        public JiraField ConvertField(dynamic field)
        {
            return new JiraField((string)field.id, (string)field.name, (string)field.description, (bool)field.custom);
        }

        public CardStatus ConvertStatus(dynamic status)
        {
            if (status == null)
                return null;

            var category = (string)status.statusCategory.key;
            return
                new CardStatus(
                (string)status.id,
                (string)status.name,
                (string)status.description,
                _jiraStatusCategoryMapping.ContainsKey(category) ?
                    _jiraStatusCategoryMapping[category] :
                    StatusCategory.Unknown);
        }

        public User ConvertUser(dynamic user)
        {
            return user == null ? null :
                new User
                {
                    Id = (string)user.key,
                    FullName = (string)user.name,
                    Email = (string)user.emailAddress
                };
        }

        public Card ConvertTicket(dynamic issue, IEnumerable<JiraField> fieldsMeta, IJiraClient client)
        {
            var mapping = new Dictionary<CardFieldMeta, object>();
            _jiraFieldMapping.ForEach(jf =>
            {
                var jiraField = fieldsMeta.FirstOrDefault(ff => ff.Name == jf.Item1);
                var fieldValue = GetPropertyValue(issue.fields, jiraField.Id);
                mapping.Add(jf.Item2, jf.Item3(fieldValue));
            });

            var card = new Card((string)issue.key, mapping);

            foreach (var hist in issue.changelog.histories)
            {
                foreach (var item in hist.items)
                {
                    var match = (JiraField)null;
                    switch ((string)item.fieldtype)
                    {
                        case "custom":
                            match = fieldsMeta.FirstOrDefault(f => f.Name == (string)item.field);
                            break;
                        case "jira":
                            match = fieldsMeta.FirstOrDefault(f => f.Id == (string)item.field);
                            break;
                    }

                    if (match == null)
                    {
                        // if the field is not found, this means we are not interesting in this
                        _logger.Debug($"Field {(string) item.field} not a known field within by client !!, skipping.");
                        continue; 
                    }

                    var jiraFieldName = match.Name;
                    var fieldMapping = _jiraFieldMapping.FirstOrDefault(t => t.Item1 == jiraFieldName);
                    if (fieldMapping == null)
                    {
                        // agian, if not found, this is not a field we are interest in
                        _logger.Debug($"Field {jiraFieldName} not mapped, skipping.");
                        continue;
                    }

                    var hi = new HistoryItem
                    {
                        By = client.GetUser((string) hist.author.key),
                        Field = fieldMapping.Item2,
                        On = (DateTime)hist.created,
                        From = fieldMapping.Item4((string) item.from, (string) item.fromString, client),
                        To = fieldMapping.Item4((string) item.to, (string) item.toString, client),
                    };

                    card.History.Add(hi);
                }
            }

            return card;
        }

        private CardType IdentifyCardType(string typeName)
        {
            if (!_jiraTicketTypeMapping.ContainsKey(typeName))
                return CardType.Unknown;

            return _jiraTicketTypeMapping[typeName];
        }

        private static IEnumerable<Sprint> ExtractSprintDetailsFromString(object data)
        {
            var list = new List<Sprint>();
            if (!(data is JArray sprints)) return list;

            foreach (var sprint in sprints)
            {
                var sprintInfo = sprint.Value<string>();
                var regEx = Regex.Match(sprintInfo, @"\[id=(\d+),rapidViewId=(.+),state=(.+),name=(.+),startDate=(.+),endDate=(.+),completeDate=(.+),");

                if (!regEx.Success)
                    continue;

                list.Add(new Sprint
                {
                    Id = regEx.Groups[1].Value,
                    BoardId = regEx.Groups[2].Value,
                    Name = regEx.Groups[4].Value,
                    StartDate = DateTime.Parse(regEx.Groups[5].Value),
                    EndDate = regEx.Groups[6].Value == "<null>" ? (DateTime?)null : DateTime.Parse(regEx.Groups[6].Value),
                    CompletionDate = regEx.Groups[7].Value == "<null>" ? (DateTime?)null : DateTime.Parse(regEx.Groups[7].Value),
                });
            }
            return list;
        }
        
        #region Helpers

        /// <summary>
        /// Get a property by name on a dynamically resolved object
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object GetPropertyValue(dynamic fields, string propertyName)
        {
            var value = fields as JValue;
            var container = fields as JContainer;
            var children = value != null ? value.Children() : container.Children();

            foreach (var child in children)
                if (child is JProperty prop && prop.Name == propertyName)
                    return prop.Value;

            return null;
        }

        /// <summary>
        /// Extract a list of object of type T from a dynamic object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<T> ExtractList<T>(dynamic obj)
        {
            var list = new List<T>();

            foreach (var lbl in obj)
                list.Add((T)lbl);

            return list;
        }

        #endregion
    }
}
