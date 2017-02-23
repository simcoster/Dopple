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
       public static TestParent SelectMe()
        {
            return new TestChild();
        }
    }

    public class TestParent
    {
        public int Count { get; set; }
        public TestParent()
        {
            Count = 5;
        }
    }

    public class TestChild : TestParent
    {
        public int Blah { get; set; }
        public TestChild()
        {
            Blah = Count;
            Count = Blah;
        }
    }
}