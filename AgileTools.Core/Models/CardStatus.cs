using System;

namespace AgileTools.Core.Models
{
    public enum StatusCategory { New, InProgress, Final, Unknown }

    public class CardStatus
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public StatusCategory Category { get; protected set; }

        public CardStatus(string id, string name, string description, StatusCategory category)
        {
            Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof(id));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Description = description;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Name} ({Category})";
        }
    }
}