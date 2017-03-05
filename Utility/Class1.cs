using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public abstract class Class1
    {
        public int TestConditionals(int number, int multSoFar)
        {
            if (number == multSoFar)
            {
                return TestConditionals(number + 1, multSoFar + 1);
            }
            else
            {
                return 0;
            }
            return multSoFar;
        }
        public abstract int CalcAtzeret(int number, int multSoFar);

        public int CallVirtual()
        {
            return new ChildClass().CalcAtzeret(6, 7);
        }

       

        //public static IEnumerable<bool> SelectMe(List<int> arr)
        //{
        //    return arr.Select(x => x > 7).ToArray();
        //}

        //public static int DoThing(bool isTrue)
        //{
        //    TestParent myObj2;
        //    if (isTrue)
        //    {
        //        myObj2 = new TestChild();
        //    }
        //    else
        //    {
        //        myObj2 = new TestAnotherChild();
        //    }
        //    return myObj2.Overridable(7);
        //}
        //public IntClass TestOrder()
        //{
        //    return new IntClass(4);
        //}
        //public int Ordered(int first, int second)
        //{
        //    return first - second;
        //}
    }

    public class ChildClass : Class1
    {
        public override int CalcAtzeret(int number, int multSoFar)
        {
            if (number == 1)
            {
                return multSoFar;
            }
            return CalcAtzeret(number - 1, multSoFar * number);
        }
    }
}