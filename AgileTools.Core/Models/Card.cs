using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgileTools.Core.Models
{
    /// <summary>
    /// Mostly used for card history analysis, this lets access well known fields on a card
    /// </summary>
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
        #region Private

        private Dictionary<CardFieldMeta, object> _fieldMapping;

        #endregion

        #region Card data fields

        public string Id { get; protected set; }
        public string Title { get => (string)this[CardFieldMeta.Title];  }
        public string Description { get => (string) this[CardFieldMeta.Description ]; }
        public DateTime CreationDate { get => (DateTime) this[CardFieldMeta.Created ];  }
        public DateTime? ClosureDate { get => (DateTime?) this[CardFieldMeta.ResolutionDate ]; }
        public DateTime? DueDate { get => (DateTime?) this[CardFieldMeta.DueDate ];  }
        public string Status { get => (string)this[CardFieldMeta.Status]; }
        public bool IsFlagged { get => (bool) this[CardFieldMeta.Flagged]; }
        public string EpicKey { get => (string)this[CardFieldMeta.EpicId]; }
        public int? SprintId { get => (int?)this[CardFieldMeta.Sprint]; }
        public string Rank { get => (string)this[CardFieldMeta.Rank]; }
        public IEnumerable<string> Labels { get => (IEnumerable<string>)this[CardFieldMeta.Labels]; }

        #endregion

        public IList<HistoryItem> History { get;  set; }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public Card(string id, Dictionary<CardFieldMeta, object> data)
        {
            Id = id;
            History = new List<HistoryItem>();
            _fieldMapping = new Dictionary<CardFieldMeta, object>(data);
        }

        /// <summary>
        /// Retrieve a field value 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a field value from a card as it was at a specific date
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="card"></param>
        /// <param name="field"></param>
        /// <param name="atDate"></param>
        /// <returns></returns>
        public T GetFieldAtDate<T>(CardFieldMeta field, DateTime atDate)
        {
            if (atDate < this.CreationDate)
                return default(T);

            var lastChangesOnFieldPriorDate = this.History
                .Where(h => h.Field == field && h.On <= atDate)
                .OrderByDescending(h => h.On)
                .FirstOrDefault();

            var firstChangesMadeAfterDate = this.History
                .Where(h => h.Field == field && h.On > atDate)
                .OrderBy(h => h.On)
                .FirstOrDefault();

            if (lastChangesOnFieldPriorDate != null)
                return lastChangesOnFieldPriorDate.To.ChangeTo<T>();

            if (firstChangesMadeAfterDate != null)
                return firstChangesMadeAfterDate.From.ChangeTo<T>();

            return this[field].ChangeTo<T>();
        }

        public override string ToString()
        {
            return $"{Id}/{Status} - {Title}";
        }
    }
}
