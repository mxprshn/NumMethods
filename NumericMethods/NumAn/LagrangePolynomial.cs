using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace Interpolation
    {
        public class LagrangePolynomial : IInterpolationPolynomial
        {
            private (double x, double y)[] dataPoints;

            public LagrangePolynomial((double x, double y)[] dataPoints)
            {
                this.dataPoints = new (double x, double y)[dataPoints.Length];
                Array.Copy(dataPoints, this.dataPoints, dataPoints.Length);
            }

            public Func<double, double> ToFunction()
            {
                var dataPointsCopy = new (double x, double y)[dataPoints.Length];
                Array.Copy(dataPoints, dataPointsCopy, dataPoints.Length);

                return x =>
                {
                    var result = 0.0;

                    for (var i = 0; i < dataPointsCopy.Length; ++i)
                    {
                        var sumTerm = dataPointsCopy[i].y;

                        for (var j = 0; j < dataPointsCopy.Length; ++j)
                        {
                            if (i != j)
                            {
                                sumTerm *= (x - dataPointsCopy[j].x) / (dataPointsCopy[i].x - dataPointsCopy[j].x);
                            }
                        }

                        result += sumTerm;
                    }

                    return result;
                };
            }
        }
    } 
}
