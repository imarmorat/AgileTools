using System;
using System.Collections.Generic;
using System.Text;

namespace JiraTools.Core.Models
{
    public struct JiraCommonFields
    {
        public const string Id = "key";
        public const string Description = "description";
        public const string FixVersion = "Fix Version/s";
        public const string Status = "Status";
        public const string CreationDate = "Created";
        public const string Resolution = "Resolution";
        public const string Labels = "Labels";

        public const string Points = "Story Points";
        public const string EpicId = "Epic Link";
        public const string EpicName = "Epic Name";
        public const string Rank = "Rank";
        public const string Flagged = "Flagged";
        public const string SprintId = "Sprint";
    }
}
