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
        //public static int SelectMe()
        //{
        //    var myObj2 = new TestChild();
        //    Console.WriteLine("Interrupting");
        //    return DoThing(myObj2);
        //}
        //public static int DoThing(TestChild myObj)
        //{
        //    var myObj2 = new TestChild();
        //    myObj2 = myObj;
        //    Console.WriteLine("Interrupting");
        //    return myObj2.NotStatic();
        //}
        public static bool[] LinqSelect(int[] arr)
        {
            return arr.Select(x => x > 7).ToArray();
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

    public struct TestChild
    {
        public int Blah { get; set; }
        //public TestChild()
        //{
        //    Blah = Count;
        //    Count = Blah;
        //}
        public int NotStatic()
        {
            return Blah;
        }
    }
}