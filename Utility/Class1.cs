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
        static void IfElse(int a)
        {
            for (int i = 0; i < 5; i++)
            {
                if (a==1)
                {
                    a--;
                }
                else if (a==2)
                {
                    a++;
                }
            }
            a = a*5;
        }

        static void SwitchCase(int a)
        {
            for (int i = 0; i < 5; i++)
            {
                switch (a)
                {
                    case 1:
                        a--;
                        break;
                    case 2:
                        a++;
                        break;
                }
            }
            a = a * 5;
        }
    }
}
