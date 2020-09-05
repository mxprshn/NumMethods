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

        public List<(double, double)> Separate((double start, double end) interval, int stepNumber)
        {
            // Check interval ends for being finite


            if (interval.start >= interval.end)
            {
                throw new RootFinderException("Отделение корней", "Начало исходного интервала должно быть меньше конца");
            }

            if (stepNumber < 2)
            {
                throw new RootFinderException("Отделение корней", "Число шагов должно быть больше 1");
            }

            var result = new List<(double, double)>();

            var stepLength = (interval.end - interval.start) / stepNumber;

            var x1 = interval.start;
            var x2 = x1 + stepLength;

            var y1 = function(x1);
            var y2 = function(x2);

            for (var i = 1; i <= stepNumber; ++i)
            {
                if (i == stepNumber)
                {
                    x2 = interval.end;
                    y2 = function(interval.end);
                }

                // Check for zero separately.

                if (y1 * y2 <= 0)
                {
                    result.Add((x1, x2));
                }

                
            }

            while (!x2.RoughlyEquals(interval.end))
            {

            }

            return null;
        }

        public IntervalResult Bisection((double start, double end) interval)
        {
            return null;
        }

        public IntervalResult Newtons((double start, double end) interval)
        {
            return null;
        }

        public IntervalResult NewtonsModified((double, double) interval)
        {
            return null;
        }

        public IntervalResult Secant((double, double) interval)
        {
            return null;
        }
    }
}
