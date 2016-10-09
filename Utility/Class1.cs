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
        public static int Caller(int a, int b)
        {
            int c = a + b;
            int d = a - b;
            return Callee(c, d);
        }

        public static int Callee(int a, int b)
        {
            return a - b;
        }

        public static int Inlined(int a, int b)
        {
            int c = a + b;
            int d = a - b;
            return c - d; 
        }
    }
}
