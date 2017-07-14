using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core.Models
{
    public class CardResolution
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is CardResolution ct && ct.Name == this.Name;
        }

        public static CardResolution CompletedSuccessfully = new CardResolution { Name = "Completed Successfully" };
        public static CardResolution Cancelled = new CardResolution { Name = "Cancelled" };
        public static CardResolution Unknown = new CardResolution { Name = "(Unknown)" };
    }
}
