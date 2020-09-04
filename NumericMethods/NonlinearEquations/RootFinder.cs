using System;
using System.Collections.Generic;
using System.Text;

namespace NonlinearEquations
{
    class RootFinder
    {
        private Func<double, double> function;
        public RootFinder(Func<double, double> function)
        {
            this.function = function;
        }

        public static List<(double, double)> Separate((double start, double end) interval, int stepNumber)
        {
            if (interval.start >= interval.end)
            {
                throw new RootFinderException("Отделение корней", "Начало исходного интервала должно быть меньше конца");
            }

            return null;
        }

        public static IntervalResult Bisection((double start, double end) interval)
        {
            return null;
        }

        public static IntervalResult Newtons((double start, double end) interval)
        {
            return null;
        }

        public static IntervalResult NewtonsModified((double, double) interval)
        {
            return null;
        }

        public static IntervalResult Secant((double, double) interval)
        {
            return null;
        }
    }
}
