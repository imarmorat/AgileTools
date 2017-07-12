using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core
{
    public static class TypeExtension
    {
        public static T ChangeTo<T>(this IConvertible obj)
        {
            // https://stackoverflow.com/questions/793714/how-can-i-fix-this-up-to-do-generic-conversion-to-nullablet
            Type t = typeof(T);
            Type u = Nullable.GetUnderlyingType(t);

            if (u != null)
            {
                return (obj == null) ? default(T) : (T)Convert.ChangeType(obj, u);
            }
            else
            {
                return (T)Convert.ChangeType(obj, t);
            }
        }
    }
}
