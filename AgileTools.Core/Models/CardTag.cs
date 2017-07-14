using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core.Models
{
    public struct CardTag
    {
        public string Value { get; set; }
        public CardTagCategory Category {get;set;}
    }

    public class CardTagCategory
    {
        public string Id { get; protected set; }

        public CardTagCategory(string id)
        {
            Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof(id));
        }
    }
}
