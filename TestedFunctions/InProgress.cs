﻿using System;
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
            return;
        }

        public static void insertionSortWithHelpers(int[] array)
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
                    swapTwo(array, i, nextMinIndex);
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
            //1.Find min
            //2.Swap it with first element
            //3.Repeat starting from secong position onwards.
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

        public static int SumIntsLinq(int[] nums)
        {
            return nums.Sum();
        }
        //public static int testLinq2(List<Helper> nums)
        //{
        //    return nums.Sum(x => x.Number);
        //}
        //public static int TestSumObjectMemeberForeachLoop(List<Helper> nums)
        //{
        //    int sum = 0;
        //    foreach (var num in nums)
        //    {
        //        sum += num.Number;
        //    }
        //    return sum;
        //}

        public static int TestSumBiggerThan7ForLoop(int[] nums)
        {
            int sum = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] > 7)
                {
                    sum += nums[i];
                }
            }
            return sum;
        }

        public static int SumIntsWhereBiggerThan7Linq(int[] nums)
        {
            return nums.Where(x => x > 7).Sum();
        }

        static public void DoMerge(int[] numbers, int left, int mid, int right)
        {
            int[] temp = new int[25];
            int i, left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left] <= numbers[mid])
                {
                    temp[tmp_pos] = numbers[left];
                    tmp_pos++;
                    left++;
                }
                else
                {
                    temp[tmp_pos] = numbers[mid];
                    tmp_pos++;
                    mid++;
                }
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];

            for (i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }




        static public void MergeSortRec(int[] numbers, int left, int right)
        {
            int mid;

            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSortRec(numbers, left, mid);
                MergeSortRec(numbers, (mid + 1), right);

                DoMerge(numbers, left, (mid + 1), right);
            }
        }

        static public void MergeSort(int[] numbers)
        {
            MergeSortRec(numbers, numbers.Length, 0);
        }


        static public void TestRec(int[] arr, int left, int right)
        {
            var lefttemp = left + 5;
            var rightTemp = 4;
            TestRec(arr, lefttemp, rightTemp);
            TestRec(new int[4], lefttemp, rightTemp);
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


        public bool BinarySearch(int first, int last, int[] mynumbers, int target)
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
        public void SimpleTwoLoops()
        {

            int j = 0;
            if (j > 5)
            {
                Console.Write("blah");
            }
        }
        public int SumArray(int[] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        public static int SumArrayInStages(int[] array)
        {
            int sumEven = 0;
            for (int i = 0; i < array.Length; i += 2)
            {
                sumEven += array[i];
            }

            int sumOdd = 0;
            for (int i = 1; i < array.Length; i += 2)
            {
                sumOdd += array[i];
            }

            return sumEven + sumOdd;

        }

        public static void QuickSortMiddle(int[] arr)
        {
            QuicksortMiddleRec(arr, 0, arr.Length);
        }

        public static void QuicksortMiddleRec(int[] elements, int left, int right)
        {
            int i = left, j = right;
            int pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i] < pivot)
                {
                    i++;
                }

                while (elements[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    //Swap
                    int tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            //Recursive calls
            if (left < j)
            {
                QuicksortMiddleRec(elements, left, j);
            }

            if (i < right)
            {
                QuicksortMiddleRec(elements, i, right);
            }
        }



        static void BubbleSort(int[] number)
        {
            bool flag = true;
            int temp;
            int numLength = number.Length;
            //sorting an array
            for (int i = 1; (i <= (numLength - 1)) && flag; i++)
            {
                flag = false;
                for (int j = 0; j < (numLength - 1); j++)
                {
                    if (number[j + 1] > number[j])
                    {
                        temp = number[j];
                        number[j] = number[j + 1];
                        number[j + 1] = temp;
                        flag = true;
                    }
                }
            }
        }
    }

    public class Helper
    {
        public int Number;
        public Helper anotherHelper;
        public Helper()
        {
            Number = 4;
        }
        //neeed to add a real exmaple
        //object with object field in it and with 2 inheriting implementing 2 a virtual method
    }
}
