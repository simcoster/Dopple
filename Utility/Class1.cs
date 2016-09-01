using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class Class1
    {
        public static int TwoVarsPath(int a, int b)
        {
            var c = a;
            var d = b;
            c = c + 3;
            int g = 5;
            g--;
            g++;
            c = -c;
            d = d + 4;
            var e = c + d;
            return e;
        }

        public string TwoStrings(string a, string b)
        {
            var c = a;
            var d = b;
            c = c.ToUpper();
            Bleep = c;
            return a;
        }

        public string Bleep { get; set; }
    }
}
