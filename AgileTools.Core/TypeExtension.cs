using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.Core
{
    public static class TypeExtension
    {
        public static T ChangeTo<T>(this object obj)
        {
            // todo: handle generic
            return (T)obj;
        }
    }
}
