﻿using Dopple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class InProgress
    {
        public static int TestConditionals(int[] a)
        {
            Helper helper1 = new Helper();
            Helper helper2 = new Helper();
            helper1.Number = 0;

            int sum = 0;
            helper1.anotherHelper = helper2;
            while (helper2.Number < a.Length)
            {
                if (sum > 20)
                {
                    sum += a[helper1.anotherHelper.Number];
                    helper2.Number++;
                }
                else
                {
                    sum += a[helper2.Number];
                    helper1.anotherHelper.Number--;
                }
                helper2.Number /= 2;
            }
            sum += 5;
            return sum;
        }
        //public static List<int> UseDelegate(int[] nums)
        //{
        //    return nums.Select(x => x++).ToList();
        //}
        //public static int testLinq(List<int> nums)
        //{
        //    int sum = 0;
        //    foreach (var num in Enumerable.Where(nums,(x => x > 6)))
        //    {
        //        sum += num;
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
