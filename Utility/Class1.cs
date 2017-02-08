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

        public static bool BinarySearch(int first, int last, int[] mynumbers, int target)
        {
            while (first <= last)
            {
                var mid = (first + last) / 2;

                if (target < mynumbers[mid])
                {
                    first = mid + 1;
                }

                if (target > mynumbers[mid])
                {
                    last = mid - 1;
                }

                else
                {
                    return true;
                }
            }
            return false;
        }
        static void QuickSortLeftPivot(int[] a)
        {
            QuickSortLeftPivotRec(a, 0, a.Length - 1);
        }


        static void QuickSortLeftPivotRec(int[] a, int start, int end)
        {
            if (start >= end)
            {
                return;
            }

            int num = a[start];

            int i = start, j = end;

            while (i < j)
            {
                while (i < j && a[j] > num)
                {
                    j--;
                }

                a[i] = a[j];

                while (i < j && a[i] < num)
                {
                    i++;
                }

                a[j] = a[i];
            }

            a[i] = num;
            QuickSortLeftPivotRec(a, start, i - 1);
            QuickSortLeftPivotRec(a, i + 1, end);
        }
        public static int SumInSingleStep(int a, int b, int c)
        {
            return a + b + c;
        }
        public static int SumInTwoStep(int a, int b, int c)
        {
            return new[] { a, b, c }.Sum();
        }
        public static void insertionSort(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int nextMin = int.MaxValue;
                int nextMinIndex = -1;
                for (int j = i; j < array.Length; j++)
                {
                    if (array[j] < nextMin)
                    {
                        nextMin = array[j];
                        nextMinIndex = j;
                    }
                }
                if (nextMin < array[i])
                {
                    int temp = array[i];
                    array[i] = array[nextMinIndex];
                    array[nextMinIndex] = temp;
                }
            }
        }
        public static void insertionSortWithHelpers(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int min;
                int minIndex;
                findMinAndIndex(array, i, out min, out minIndex);
                if (min < array[i])
                {
                    swapTwo(array, i, minIndex);
                }
            }
        }

        public static void swapTwo(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static void findMinAndIndex(int[] array, int startIndex, out int minValue, out int minIndex)
        {
            int currentMin = int.MaxValue;
            int currentMinIndex = -1;
            for (int i = startIndex; i < array.Length; i++)
            {
                if (array[i] < currentMin)
                {
                    currentMin = array[i];
                    currentMinIndex = i;
                }
            }
            minValue = currentMin;
            minIndex = currentMinIndex;
        }

        public int[] SelectionSort(int[] arr)
        {
            //1. Find min
            //2. Swap it with first element
            //3. Repeat starting from secong position onwards.
            int _min = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                _min = i;
                for (int j = i; j < arr.Length; j++)
                {
                    if (arr[j] < arr[_min])
                        _min = j;
                }
                int _temp = arr[i];
                arr[i] = arr[_min];
                arr[_min] = _temp;
            }
            return arr;
        }


    }
}