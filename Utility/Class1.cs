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
        public static int DoThing(bool isTrue)
        {
            TestParent myObj2;
            if (isTrue)
            {
                myObj2 = new TestChild();
            }
            else
            {
                myObj2 = new TestAnotherChild();
            }
            return myObj2.Overridable(7);
        }
        //public static bool[] LinqSelect(int[] arr)
        //{
        //    return arr.Select(x => x > 7).ToArray();
        //}
    }

    public class TestParent
    {
        public int Count { get; set; }
        public virtual int Overridable(int arg)
        {
            return 5;
        }
    }

    public class TestChild : TestParent
    {
        public int Blah { get; set; }
        public override int Overridable(int arg)
        {
            return arg + base.Overridable(arg);
        }
        //public TestChild()
        //{
        //    Blah = Count;
        //    Count = Blah;
        //}

    }

    public class TestAnotherChild : TestParent
    {
        public override int Overridable(int arg)
        {
            return arg;
        }
        //public TestChild()
        //{
        //    Blah = Count;
        //    Count = Blah;
        //}

    }
}