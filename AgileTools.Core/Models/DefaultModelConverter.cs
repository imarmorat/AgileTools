using AgileTools.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgileTools.Core.Models.Card;

namespace AgileTools.Core.Models
{
    /// <summary>
    /// This model converter is tested for JIRA 7.3.0 and below (havent tested below 7.0)
    /// </summary>
    public class DefaultModelConverter : IModelConverter
    {
        #region Private

        private List<Tuple<string, CardFieldMeta, Func<dynamic, object>>> _jiraFieldMapping = new List<Tuple<string, CardFieldMeta, Func<dynamic, object>>>
         {
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Summary", CardFieldMeta.Title, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Description", CardFieldMeta.Description, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Created", CardFieldMeta.Created, o=> (DateTime) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Resolved", CardFieldMeta.ResolutionDate, o=> (DateTime?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Due Date", CardFieldMeta.DueDate, o=> (DateTime?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Story Points", CardFieldMeta.Points, o=> (int?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Sprint", CardFieldMeta.Sprint, o=> (int?) null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Flagged", CardFieldMeta.Flagged, o=> o != null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Resolution", CardFieldMeta.Resolution, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Status", CardFieldMeta.Status, o=> (string) o.name),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Epic Link", CardFieldMeta.EpicId, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Rank", CardFieldMeta.Rank, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Labels", CardFieldMeta.Labels, o=> ExtractList<string>(o) ),
        };
        private Dictionary<string, StatusCategory> _jiraStatusCategoryMapping = new Dictionary<string, StatusCategory>
        {
            { "new", StatusCategory.New },
            { "indeterminate", StatusCategory.InProgress },
            { "done", StatusCategory.Final }
        };

        #endregion

        public JiraField ConvertField(dynamic field)
        {
            return new JiraField((string)field.id, (string)field.name, (string)field.description, (bool)field.custom);
        }

        public CardStatus ConvertStatus(dynamic status)
        {
            var category = (string)status.statusCategory.key;
            return new CardStatus(
                (string)status.id, 
                (string)status.name, 
                (string)status.description, 
                _jiraStatusCategoryMapping.ContainsKey(category) ?
                    _jiraStatusCategoryMapping[category] :
                    StatusCategory.Unknown);
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
