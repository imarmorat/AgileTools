using AgileTools.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AgileTools.Core.Models.Card;

namespace AgileTools.Core.Models
{
    /// <summary>
    /// This model converter is tested for JIRA 7.3.0 and below (havent tested below 7.0)
    /// </summary>
    public class DefaultModelConverter : IModelConverter
    {
        #region Protected

        protected List<Tuple<string, CardFieldMeta, Func<dynamic, object>>> _jiraFieldMapping;
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
            _jiraFieldMapping = new List<Tuple<string, CardFieldMeta, Func<dynamic, object>>>
         {
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Summary", CardFieldMeta.Title, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Issue Type", CardFieldMeta.Type, o=> IdentifyCardType(o)),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Description", CardFieldMeta.Description, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Created", CardFieldMeta.Created, o=> (DateTime) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Resolved", CardFieldMeta.ResolutionDate, o=> (DateTime?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Due Date", CardFieldMeta.DueDate, o=> (DateTime?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Story Points", CardFieldMeta.Points, o=> (int?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Sprint", CardFieldMeta.Sprint, o=> ExtractSprintDetailsFromString(o)),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Flagged", CardFieldMeta.Flagged, o=> o != null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Resolution", CardFieldMeta.Resolution, o=> o != null ? (string) o.name : null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Status", CardFieldMeta.Status, o=> ConvertStatus(o) ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Epic Link", CardFieldMeta.EpicId, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Rank", CardFieldMeta.Rank, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Labels", CardFieldMeta.Labels, o=> ExtractList<string>(o) ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Assignee", CardFieldMeta.Assignee, o=> ConvertUser(o) ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Reporter", CardFieldMeta.Reporter, o=> ConvertUser(o) ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Creator", CardFieldMeta.Creator, o=> ConvertUser(o) ),
            };
        }

        private static IEnumerable<Sprint> ExtractSprintDetailsFromString(object data)
        {
            var list = new List<Sprint>();
            if (!(data is JArray sprints))  return list;

            foreach (var sprint in sprints)
            {
                var sprintInfo = sprint.Value<string>();
                var regEx = Regex.Match(sprintInfo, @"\[id=(\d+),rapidViewId=(.+),state=(.+),name=(.+),startDate=(.+),endDate=(.+),completeDate=(.+),");

                if (!regEx.Success)
                    continue;

                list.Add( new Sprint
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

        public static User ConvertUser(dynamic user)
        {
            return user == null ? null :
                new User
                {
                    Id = (string)user.key,
                    FullName = (string)user.name,
                    Email = (string)user.emailAddress
                };
        }

        public Card ConvertTicket(dynamic issue, IEnumerable<JiraField> fieldsMeta)
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
                    var hi = new HistoryItem
                    {
                        // author
                        On = (DateTime)hist.created,
                        From = (string)item.from,
                        FromStr = (string)item.fromString,
                        To = (string)item.to,
                        ToStr = (string)item.toString
                    };

                    var jiraFieldName = string.Empty;
                    switch ((string)item.fieldtype)
                    {
                        case "custom":
                            jiraFieldName = fieldsMeta.FirstOrDefault(f => f.Name == (string)item.field).Name;
                            break;
                        case "jira":
                            jiraFieldName = fieldsMeta.FirstOrDefault(f => f.Id == (string)item.field).Name;
                            break;
                        default: throw new Exception($"Unknown field type during history decomposition: {item.field}");
                    }
                    hi.Field = _jiraFieldMapping.FirstOrDefault(t => t.Item1 == jiraFieldName).Item2;
                    card.History.Add(hi);
                }
            }

            return card;
        }

        private CardType IdentifyCardType(dynamic ct)
        {
            if (ct == null || !_jiraTicketTypeMapping.ContainsKey((string)ct.name))
                return CardType.Unknown;

            return _jiraTicketTypeMapping[(string)ct.name];
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
