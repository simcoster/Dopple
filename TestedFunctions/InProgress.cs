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
        static int blah =7;
        public static List<int> UseDelegate(int[] nums)
        {
            return nums.Select(x => x++).ToList();
        }
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
        public string Text;
        public string Text2;
        public Helper()
        {
            //Number = 4;
            //Text2 = "Blaaa";
        }
    }
}
