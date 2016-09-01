namespace Utility
{
    internal class IntClass
    {
        public int TheInt { get; internal set; }

        public static void PlusOne()
        {
            int a = 1;
            a++;
        }

        public static int PlusOneRet()
        {
            int a = 1;
            a++;
            return a;
        }
        public static void PlusOneOne()
        {
            int a = 1;
            a=a+1;
        }
        public static void PlusOneTwo()
        {
            int a = 1;
            ++a;
        }
    }
}