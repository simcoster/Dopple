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
        //--------------- iterative version ---------------------    
        static int FibonacciIterative(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 1;

            int prevPrev = 0;
            int prev = 1;
            int result = 0;

            for (int i = 2; i <= n; i++)
            {
                result = prev + prevPrev;
                prevPrev = prev;
                prev = result;
            }
            return result;
        }

        //--------------- naive recursive version --------------------- 
        static int FibonacciRecursive(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 1;

            return FibonacciRecursive(n - 1) + FibonacciRecursive(n - 2);
        }


        //public static void insertionSort(int[] array)
        //{
        //    for (int i = 0; i < array.Length - 1; i++)
        //    {
        //        int nextMin = int.MaxValue;
        //        int nextMinIndex = -1;
        //        for (int j = i; j < array.Length; j++)
        //        {
        //            if (array[j] < nextMin)
        //            {
        //                nextMin = array[j];
        //                nextMinIndex = j;
        //            }
        //        }
        //        if (nextMin < array[i])
        //        {
        //            int temp = array[i];
        //            array[i] = array[nextMinIndex];
        //            array[nextMinIndex] = temp;
        //        }
        //    }
        //}


        //public static void insertionSortWithHelpers(int[] array)
        //{
        //    for (int i = 0; i < array.Length - 1; i++)
        //    {
        //        int min;
        //        int minIndex;
        //        findMinAndIndex(array, i, out min, out minIndex);
        //        //int min = int.MaxValue;
        //        //int minIndex = -1;
        //        for (int j = i; j < array.Length; j++)
        //        {
        //            if (array[j] < min)
        //            {
        //                min = array[j];
        //                minIndex = j;
        //            }
        //        }
        //        if (min < array[i])
        //        {
        //            swapTwo(array, i, minIndex);
        //        }
        //    }
        //}

        //public static void swapTwo(int[] array, int i, int j)
        //{
        //    int temp = array[i];
        //    array[i] = array[j];
        //    array[j] = temp;
        //}

        //public static void findMinAndIndex(int[] array, int startIndex, out int minValue, out int minIndex)
        //{
        //    int currentMin = int.MaxValue;
        //    int currentMinIndex = -1;
        //    for (int i = startIndex; i < array.Length; i++)
        //    {
        //        if (array[i] < currentMin)
        //        {
        //            currentMin = array[i];
        //            currentMinIndex = i;
        //        }
        //    }
        //    minValue = currentMin;
        //    minIndex = currentMinIndex;
        //}

        //public static IEnumerable<bool> SelectMe(List<int> arr)
        //{
        //    return arr.Select(x => x > 7).ToList();
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

    //public class ChildClass : Class1
    //{
    //   
    //}
}