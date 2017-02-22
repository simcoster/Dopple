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
       public static IEnumerable<bool> SelectMe(int[] arr)
        {
            return arr.Select(x => x > 5);
        }
    }
}