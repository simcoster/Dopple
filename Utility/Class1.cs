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
        static public int AddRec(int[] arr, int toSearch, int startIndex)
        {
            if (arr[startIndex] == toSearch || startIndex>arr.Length)
            {
                return startIndex;
            }
            else
            {
                return  AddRec(arr, toSearch, startIndex + 1);
            }
        }

        static public int AddLoop(int[] arr, int toSearch, int startIndex)
        {
            for (int i=startIndex; i < arr.Length; i++)
            {
                if (arr[startIndex] == toSearch)
                {
                    return startIndex;
                }
            }
            return startIndex;
        }

        //static public int AddLoop(int a, int b)
        //{
        //    while (a + b < 5)
        //    {
        //        a += 2;
        //        b += 1;
        //    }
        //    return a + b;
        //}
    }
}
