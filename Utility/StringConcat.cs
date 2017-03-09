using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    class stringConcat
    {
        static string stringListConcat(List<string> strings)
        {
            string res = "";
            foreach (string s in strings)
                res += s;
            return res;
        }

        static string stringListConcatWithWhitespace(List<string> strings)
        {
            string res = "";
            foreach (string s in strings)
                res += s + " ";
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        static string stringListConcatWithSeparator(List<string> strings, string separator)
        {
            string res = "";
            foreach (string s in strings)
                res += s + separator;
            return res = res.Substring(0, res.Length - separator.Length);
        }

        static string stringListConcatWIthSeparatorLinq(List<string> strings, string separator)
        {
            return strings.Aggregate((acc, x) => acc + separator + x);
        }

        static string stringListTwoLevelConcatLinq(List<List<string>> stringLists, string separator)
        {
            List<string> strings = stringLists.Select(l => l.Aggregate((acc, x) => acc + separator + x)).ToList();
            return strings.Aggregate((acc, x) => acc + separator + x);
        }

    }
}