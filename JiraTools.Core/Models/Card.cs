using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiraTools.Core.Models
{
    public enum CardFieldMeta
    {
         Id ,
         Title ,
         Description ,
         Status ,
         Points ,
         Resolution ,
         ResolutionDate ,
         Created ,
         DueDate ,

         Flagged ,
         Sprint ,
         EpicId ,
         Rank,
         Labels
    }

    /// <summary>
    /// We are not seperating between normal ticket and agile ticket has there is no point.
    /// The purpose of this tool is to focus on agile analysis
    /// </summary>
    public class Card
    {
        public string Id { get; set; }
        public string Title { get => (string)this[CardFieldMeta.Title];  }
        public string Description { get => (string) this[CardFieldMeta.Description ]; }
        public DateTime CreationDate; // { get => (DateTime) this[ JiraDefaultFields.Created ]; set => this[ JiraDefaultFields.Created] = value; }
        public DateTime? ClosureDate; // { get => (DateTime?) this[ JiraDefaultFields.ResolutionDate ]; set => this[ JiraDefaultFields.ResolutionDate] = value; }
        public DateTime? DueDate; // { get => (DateTime?) this[ JiraDefaultFields.DueDate ]; set => this[ JiraDefaultFields.DueDate] = value; }

        public bool IsFlagged { get => (bool) this[CardFieldMeta.Flagged]; }
        public string EpicKey { get => (string)this[CardFieldMeta.EpicId]; }
        public int? SprintId { get => (int?)this[CardFieldMeta.Sprint]; }
        public string Rank { get => (string)this[CardFieldMeta.Rank]; }

        public IList<HistoryItem> History { get;  set; }
        public IEnumerable<string> Labels { get => (IEnumerable<string>)this[CardFieldMeta.Labels]; }
        public IList<string> FixVersions { get; set; }

        public class HistoryItem
        {
            public CardFieldMeta Field { get; set; }
            public object From { get; set; }
            public object To { get; set; }
            public DateTime On { get; set; }
            public string FromStr { get; internal set; }
            public string ToStr { get; internal set; }
            // public string By {get;set;}

            public override string ToString()
            {
                return $"{Field} changed from [{From}/{FromStr}] to [{To}/{ToStr}] by XYZ on {On}";
            }
        }

        public class CommentItem
        {
            public string Comment { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime? LastUpdatedOn { get; set; }
        }

        private Dictionary<CardFieldMeta, object> _fieldMapping;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldsMeta"></param>
        public Card(string id)
        {
            Id = id;
            History = new List<HistoryItem>();
            FixVersions = new List<string>();
        }

        public Card(string id, Dictionary<CardFieldMeta, object> data) : this(id)
        {
            _fieldMapping = new Dictionary<CardFieldMeta, object>(data);
        }

        public object this[CardFieldMeta fieldName]
        {
            get
            {
                return _fieldMapping.ContainsKey(fieldName) ? _fieldMapping[fieldName] : null;
            }

            set
            {
                if (_fieldMapping.ContainsKey(fieldName))
                    _fieldMapping[fieldName] = value;
                else
                    _fieldMapping.Add(fieldName, value);
            }
        }

        public override string ToString()
        {
            return $"{Id} - {Title}";
        }
    }
}
