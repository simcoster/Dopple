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

        //public static bool BinarySearch(int first, int last, int[] mynumbers, int target)
        //{
        //    while (first <= last)
        //    {
        //        var mid = (first + last) / 2;

        //        if (target < mynumbers[mid])
        //        {
        //            first = mid + 1;
        //        }

        //        if (target > mynumbers[mid])
        //        {
        //            last = mid - 1;
        //        }

        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public bool BinarySearchRec(int first, int last, int[] mynumbers, int target)
        {
            if (first == last)
            {
                return mynumbers[first] == target;
            }

            var mid = (first + last) / 2;

            if (target < mynumbers[mid])
            {
                return BinarySearchRec(mid + 1, last, mynumbers, target);
            }
            if (target > mynumbers[mid])
            {
                return BinarySearchRec(first, mid - 1, mynumbers, target);
            }
            else
            {
                return true;
            }
        }
    }
}
