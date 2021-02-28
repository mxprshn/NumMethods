using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public class DoubleUtils
    {
        public static void Swap(ref double one, ref double another)
        {
            var temp = one;
            one = another;
            another = temp;
        }
    }
}
