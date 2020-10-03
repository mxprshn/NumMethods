using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace Interpolation
    {
        public class NewtonsPolynomial
        {
            private double[][] dividedDifferences;
            private (double x, double y)[] dataPoints;

            public NewtonsPolynomial((double x, double y)[] dataPoints)
            {
                dividedDifferences = new double[dataPoints.Length][];
                this.dataPoints = new (double x, double y)[dataPoints.Length];
                Array.Copy(dataPoints, this.dataPoints, dataPoints.Length);

                for (var i = 0; i < dividedDifferences.Length; ++i)
                {
                    dividedDifferences[i] = new double[dividedDifferences.Length - i];

                    for (var j = 0; j < dividedDifferences[i].Length; ++j)
                    {
                        dividedDifferences[i][j] = ComputeDividedDifference(j, j + i);
                    }
                }
            }

            private double ComputeDividedDifference(int index0, int indexK)
            {
                if (index0 == indexK)
                {
                    return dataPoints[index0].y;
                }

                return (dividedDifferences[indexK - index0 - 1][index0 + 1] - dividedDifferences[indexK - index0 - 1][index0]) /
                    (dataPoints[indexK].x - dataPoints[index0].x);
            }

            public Func<double, double> ToFunction()
            {
                var coefficients = new double[dataPoints.Length];
                var xPoints = new double[dataPoints.Length];

                for (var i = 0; i < coefficients.Length; ++i)
                {
                    coefficients[i] = dividedDifferences[dividedDifferences.Length - i - 1][0];
                    xPoints[i] = dataPoints[dataPoints.Length - i - 1].x;
                }

                return x =>
                {
                    var result = coefficients[0];

                    for (var i = 1; i < coefficients.Length; ++i)
                    {
                        result = result * (x - xPoints[i]) + coefficients[i];
                    }

                    return result;
                };
            }
        }
    }
}