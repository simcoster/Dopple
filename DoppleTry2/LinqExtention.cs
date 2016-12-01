using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    static class LinqExtention
    {
        public static IEnumerable<IEnumerable<T>> GroupBySequence<T, U>(this IEnumerable<T> seq, Func<T, U> map) where U : IEnumerable<T>
        {
            if (seq.Count() <2)
            {
                return null;
            }
            var groupedToReturn = new List<List<T>>();
            var done = new List<T>();
            foreach(var item in seq)
            {
                if (done.Contains(item))
                {
                    continue;
                }
                var group = seq.Where(x => map(item).Distinct().SequenceEqual(map(x).Distinct())).ToList();
                done.AddRange(group);
                groupedToReturn.Add(group);
            }
            return groupedToReturn;
        }
    }
}
