using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumAn
{
    namespace Differentiation
    {
        public class Differentiator
        {
            private (double x, double y)[] dataPoints;

            public Differentiator((double x, double y)[] dataPoints)
            {
                this.dataPoints = new (double x, double y)[dataPoints.Length];
                Array.Copy(dataPoints.OrderBy(value => value.x).ToArray(), this.dataPoints, dataPoints.Length);
            }

            public (double x, double y)[] FirstDerivative()
            {
                var result = new (double x, double y)[dataPoints.Length];

                for (var i = 0; i < result.Length; ++i)
                {
                    result[i] = (dataPoints[i].x, SymmetricDifference(i));
                }

                return result;
            }

            public (double x, double y)[] SecondDerivative()
            {
                var result = new (double x, double y)[dataPoints.Length - 2];

                for (var i = 1; i < dataPoints.Length - 1; ++i)
                {
                    var value = (dataPoints[i + 1].y - 2 * dataPoints[i].y + dataPoints[i - 1].y) /
                        Math.Pow(dataPoints[i + 1].x - dataPoints[i].x, 2);

                    result[i - 1] = (dataPoints[i].x, value);
                }

                return result;
            }

            private double SymmetricDifference(int index)
            {
                if (index == 0)
                {
                    if (dataPoints.Length > 2)
                    {
                        return (-3 * dataPoints[0].y + 4 * dataPoints[1].y - dataPoints[2].y) / (dataPoints[2].x - dataPoints[0].x);
                    }

                    return (dataPoints[1].y - dataPoints[0].y) / (dataPoints[1].x - dataPoints[0].x);
                }

                if (index == dataPoints.Length - 1)
                {
                    if (dataPoints.Length > 2)
                    {
                        return (3 * dataPoints[index].y - 4 * dataPoints[index - 1].y + dataPoints[index - 2].y) /
                            (dataPoints[index].x - dataPoints[index - 2].x);
                    }

                    return (dataPoints[index].y - dataPoints[index - 1].y) / (dataPoints[index].x - dataPoints[index - 1].x);
                }

                return (dataPoints[index + 1].y - dataPoints[index - 1].y) / (dataPoints[index + 1].x - dataPoints[index - 1].x);
            }
        }
    }
}