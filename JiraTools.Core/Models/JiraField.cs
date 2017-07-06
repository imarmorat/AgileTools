using System;

namespace JiraTools.Core.Models
{
    public class JiraField
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsCustom { get; protected set; }

        public JiraField(string id, string name, string description, bool isCustom)
        {
            Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof(id));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Description = description;
            IsCustom = isCustom;
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}