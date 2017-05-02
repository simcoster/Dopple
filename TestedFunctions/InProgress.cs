using Dopple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class InProgress
    {
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
                        for (int k =0; k<7; k++)
                        {
                            nextMinIndex = k;
                        }
                        nextMin = array[j];
                    }
                }
                if (nextMin < array[i])
                {
                    array[i] = array[nextMinIndex];
                }
            }
        }

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

        //public int[] SelectionSort(int[] arr)
        //{
        //    //1.Find min
        //    //2.Swap it with first element
        //    //3.Repeat starting from secong position onwards.
        //    int _min = 0;
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        _min = i;
        //        for (int j = i; j < arr.Length; j++)
        //        {
        //            if (arr[j] < arr[_min])
        //                _min = j;
        //        }
        //        int _temp = arr[i];
        //        arr[i] = arr[_min];
        //        arr[_min] = _temp;
        //    }
        //    return arr;
        //}

        //public static int TestOutParams(int a, int b)
        //{
        //    int answer;
        //    OutParams(a, b, out answer);
        //    return answer;
        //}

        //private static void OutParams(int a, int b, out int answer)
        //{
        //    answer = a + b;
        //}

        //public static List<int> UseDelegate(int[] nums)
        //{
        //    return nums.Select(x => x++).ToList();
        //}
        //public static int testLinq2(List<Helper> nums)
        //{
        //    return nums.Sum(x => x.Number);
        //}
        //public static int TestSumForeachLoop(List<Helper> nums)
        //{
        //    int sum = 0;
        //    foreach (var num in nums)
        //    {
        //        sum += num.Number;
        //    }
        //    return sum;
        //}

        //public static int TestSumForLoop(Helper[] nums)
        //{
        //    int sum = 0;
        //    for (int i =0;  i< nums.Length; i++)
        //    {
        //        sum += nums[i].Number;
        //    }
        //    return sum;
        //}

        //public static int SumMe(int[] nums)
        //{
        //    return nums.Sum(x => x * 6);
        //}

        //public static int DynamicGames(int index)
        //{
        //    int a = 5;
        //    Helper helper = new Helper();
        //    while (a > 6)
        //    {
        //        a++;
        //        helper.Number = a;
        //        a = helper.Number;
        //    }
        //    return a;
        //}
    }

    public class Helper
    {
        public int Number;
        public Helper anotherHelper;
        public Helper()
        {
            //Number = 4;
            //Text2 = "Blaaa";
        }
        //neeed to add a real exmaple
        //object with object field in it and with 2 inheriting implementing 2 a virtual method
    }
}
