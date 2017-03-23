using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestedFunctions
{
    public class InProgress
    {

        public static string SumMe(int index)
        {
            Helper[] helpers = new Helper[5];
            var tempHelper = new Helper();
            tempHelper.Number = 3;
            helpers[4] = tempHelper;
            tempHelper.Text = "blah";
            helpers[4].Text = "bleep";
            return tempHelper.Text;
        }
    }

    public class Helper
    {
        public int Number;
        public string Text;
        public string Text2;
        public Helper()
        {
            Number = 4;
            Text2 = "Blaaa";
        }
    }
}
