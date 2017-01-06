using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }
}
