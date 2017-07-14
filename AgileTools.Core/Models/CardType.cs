using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core.Models
{
    public class CardType
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is CardType ct && ct.Name == this.Name;
        }

        //
        // Basic card types
        public static CardType Story = new CardType { Name = "Story" };
        public static CardType Bug = new CardType { Name = "Bug" };
        public static CardType Enabler = new CardType { Name = "Enabler" };
        public static CardType Feature = new CardType { Name = "Feature" };
        public static CardType Unknown = new CardType { Name = "Unknown" };
    }
}
