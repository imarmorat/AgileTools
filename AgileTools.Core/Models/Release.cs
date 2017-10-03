using System;

namespace AgileTools.Core.Models
{
    public class Release
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}