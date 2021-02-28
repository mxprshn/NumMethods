using System;

namespace NumAn
{
    public static class DoubleExtensions
    {
        public static bool RoughlyEquals(this double first, double second) =>
            Math.Abs(first - second) < 0.000001;

        public static bool IsNumber(this double number) =>
            !double.IsNaN(number) && !double.IsInfinity(number);
    }
}