using System;

namespace AgileTools.Core.Models
{
    public class Sprint
    {
        public string Id { get; set; }
        public string BoardId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public override string ToString()
        {
            return $"({Id}/{BoardId}) {Name}";
        }
    }
}