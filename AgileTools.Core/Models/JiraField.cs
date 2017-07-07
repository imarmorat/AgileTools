using System;

namespace AgileTools.Core.Models
{
    /// <summary>
    /// Represent a JIRA field. Not used outside the concept of Jira Client as Card is dealing with it abstractively
    /// TODO: consider moving to Client project
    /// </summary>
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