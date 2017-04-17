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
        public static int TestOutParams(int a, int b)
        {
            int answer;
            OutParams(a, b, out answer);
            return answer;
        }

        private static void OutParams(int a, int b, out int answer)
        {
            answer = a + b;
        }
        //public static int TestConditionals(int[] a)
        //{
        //    Helper helper1 = new Helper();
        //    Helper helper2 = new Helper();
        //    Helper helper3 = new Helper();
        //    helper1.Number = 0;

        //    int sum = 0;
        //    if (a.Length > 7)
        //    {
        //        helper1.anotherHelper = helper2;
        //    }
        //    else
        //    {
        //        helper1.anotherHelper = helper3;
        //    }
        //    while (helper2.Number < a.Length)
        //    {
        //        if (sum > 20)
        //        {
        //            helper2.Number++;
        //        }
        //        else
        //        {
        //            helper1.anotherHelper.Number--;
        //        }
        //        helper3.Number /= 2;
        //    }
        //    sum += 5;
        //    return sum;
        //}
        //public static List<int> UseDelegate(int[] nums)
        //{
        //    return nums.Select(x => x++).ToList();
        //}
        //public static int testLinq(List<int> nums)
        //{
        //    return nums.Where(x => x > 6).Sum();
        //}

        //public static int TestSumRegular(List<int> nums)
        //{
        //    int sum = 0;
        //    foreach (var num in nums)
        //    {
        //        if (num > 6)
        //        {
        //            sum += num;
        //        }
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
