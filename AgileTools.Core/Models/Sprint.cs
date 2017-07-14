using System;

namespace AgileTools.Core.Models
{
    public class Sprint
    {
        public string Id { get; set; }
        public string BoardId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            return $"({Id}/{BoardId}) {Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is Sprint spr &&
                spr.Id == this.Id &&
                spr.Name == this.Name &&
                spr.BoardId == this.BoardId;
        }
    }
}