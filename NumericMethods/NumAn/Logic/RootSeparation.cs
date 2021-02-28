using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace RootFinding
    {
        public static class RootSeparation
        {
            private const int SeparationStepNumber = 270;

            public static List<(double Start, double End)> Separate(Func<double, double> function, (double Start, double End) interval)
            {
                if (!interval.Start.IsNumber() || !interval.End.IsNumber())
                {
                    throw new ArgumentException("Концы интервала должны быть вещественными числами.");
                }

                if (interval.Start >= interval.End)
                {
                    throw new ArgumentException("Начало исходного интервала должно быть меньше конца.");
                }

                var results = new List<(double Start, double End)>();
                var stepLength = (interval.End - interval.Start) / SeparationStepNumber;

                var x1 = interval.Start;
                var x2 = x1 + stepLength;

                var y1 = function(x1);
                var y2 = function(x2);

                for (var i = 1; i <= SeparationStepNumber; ++i)
                {
                    if (y1 * y2 <= 0)
                    {
                        results.Add((x1, x2));
                    }

                    if (i == SeparationStepNumber) break;

                    x1 = x2;
                    y1 = y2;

                    x2 = (i == SeparationStepNumber - 1) ? interval.End : (x1 + stepLength);
                    y2 = function(x2);
                }

                return results;
            }
        }
    }
}