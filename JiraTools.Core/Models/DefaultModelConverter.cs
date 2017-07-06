using JiraTools.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JiraTools.Core.Models.Card;

namespace JiraTools.Core.Models
{
    public class DefaultModelConverter : IModelConverter
    {
        public JiraField ConvertField(dynamic field)
        {
            return new JiraField((string)field.id, (string)field.name, (string)field.description,  (bool)field.custom);
        }

        public CardStatus ConvertStatus(dynamic status)
        {
            return new CardStatus((string)status.id, (string)status.name, (string)status.description, StatusCategory.Unknown);
        }

        public Card ConvertTicket(dynamic issue, IEnumerable<JiraField> fieldsMeta)
        {
            var jiraFieldMapping = new List<Tuple<string, CardFieldMeta, Func<dynamic,object>>>
            {
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Summary", CardFieldMeta.Title, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Description", CardFieldMeta.Description, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Story Points", CardFieldMeta.Points, o=> (int?) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Sprint", CardFieldMeta.Sprint, o=> (int?) null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Flagged", CardFieldMeta.Flagged, o=> o != null),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Resolution", CardFieldMeta.Resolution, o=> (string) o),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Status", CardFieldMeta.Status, o=> (string) o.name),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Epic Link", CardFieldMeta.EpicId, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Rank", CardFieldMeta.Rank, o=> (string) o ),
                new Tuple<string, CardFieldMeta, Func<dynamic, object>>("Labels", CardFieldMeta.Labels, o=>
                {
                    var list = new List<string>();

                    foreach (var lbl in o)
                        list.Add( (string) lbl );

                    return list;
                    }),
            };

            // for all non custom fields
            var mapping = new Dictionary<CardFieldMeta, object>();
            jiraFieldMapping.ForEach(jf =>
            {
                var jiraField = fieldsMeta.FirstOrDefault(ff => ff.Name == jf.Item1);
                var fieldValue =  GetPropertyValue(issue.fields, jiraField.Id);
                mapping.Add(jf.Item2, jf.Item3(fieldValue));
            });

            var cardId = (string)issue.key;
            var card = new Card(cardId, mapping);

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
                    switch((string) item.fieldtype)
                    {
                        case "custom":
                            jiraFieldName = fieldsMeta.FirstOrDefault(f => f.Name == (string)item.field).Name;
                            break;
                        case "jira":
                            jiraFieldName = fieldsMeta.FirstOrDefault(f => f.Id == (string)item.field).Name;
                            break;
                        default: throw new Exception($"Unknown field type during history decomposition: {item.field}");
                    }
                    hi.Field = jiraFieldMapping.FirstOrDefault(t=>t.Item1 == jiraFieldName).Item2;
                    card.History.Add(hi);
                }
            }


            foreach (var fv in issue.fields.versions)
                card.FixVersions.Add((string) fv);

            return card;
        }

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

    }
}
