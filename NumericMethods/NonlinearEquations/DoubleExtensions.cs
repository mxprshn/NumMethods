using System;
using System.Collections.Generic;
using System.Text;

namespace NonlinearEquations
{
    static class DoubleExtensions
    {
        public static bool RoughlyEquals(this double first, double second) =>
            Math.Abs(first - second) < 0.00001;
    }
}
