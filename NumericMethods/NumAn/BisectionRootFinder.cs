using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace RootFinding
    {
        public class BisectionRootFinder : IRootFinder
        {
            private Func<double, double> function;

            public BisectionRootFinder(Func<double, double> function)
            {
                this.function = function;
            }

            public IntervalResult FindRoots((double Start, double End) interval, double epsilon)
            {
                if (!interval.Start.IsNumber() || !interval.End.IsNumber())
                {
                    throw new ArgumentException("Концы интервала должны быть вещественными числами.");
                }

                if (epsilon <= 0)
                {
                    throw new ArgumentException("Величина эпсилон должна быть положительным числом.");
                }

                if (interval.Start >= interval.End)
                {
                    throw new ArgumentException("Начало исходного интервала должно быть меньше конца.");
                }

                if (function(interval.Start) * function(interval.End) >= 0)
                {
                    throw new ArgumentException("Значения функции на концах интервала должны иметь разные знаки.");
                }

                var start = interval.Start;
                var end = interval.End;
                var center = (start + end) / 2;
                var length = end - start;
                var yCenter = function(center);

                var result = new IntervalResult { InitialApproximations = new List<double>() { center }, IterationCount = 1 };

                while (Math.Abs(yCenter) > epsilon && length > 2 * epsilon)
                {
                    if (function(start) * yCenter <= 0)
                    {
                        end = center;
                    }
                    else
                    {
                        start = center;
                    }

                    center = (start + end) / 2;
                    yCenter = function(center);
                    length = end - start;

                    ++result.IterationCount;
                }

                result.AbsoluteResidual = Math.Abs(yCenter);
                result.Root = center;

                return result;
            }
        }
    }
}