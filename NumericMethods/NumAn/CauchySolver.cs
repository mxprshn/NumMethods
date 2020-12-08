using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NumAn
{
    public class CauchySolver
    {
        private double[] nodes;
        private double[] firstTaylors;
        private double[] derivatives;

        private readonly Func<double, double, double> derivative;
        private readonly double targetX;
        private readonly double targetY;
        private readonly double segmentLength;
        private readonly int segmentNumber;

        public CauchySolver(Func<double, double, double> derivative, double targetX, double targetY, double segmentLength, int segmentNumber, double[] derivatives)
        {            
            if (derivatives.Length < 5)
            {
                throw new ArgumentException("Число значений производных функции должно быть не меньше 5.");
            }

            this.derivative = derivative;
            this.segmentLength = segmentLength;
            this.segmentNumber = segmentNumber;
            this.targetX = targetX;
            this.targetY = targetY;

            nodes = new double[segmentNumber + 1];

            for (var i = -2; i <= segmentNumber - 2; ++i)
            {
                nodes[i + 2] = targetX + i * segmentLength;
            }

            this.derivatives = new double[derivatives.Length];
            Array.Copy(derivatives, this.derivatives, derivatives.Length);
        }

        public double[] Taylor()
        {
            var factorial = 1;
            var coefficients = new double[derivatives.Length];

            for (var i = 0; i < derivatives.Length; ++i)
            {
                coefficients[i] = (derivatives[i] / factorial);
                factorial *= i + 1;
            }

            var result = new double[nodes.Length];

            for (var i = 0; i < nodes.Length; ++i)
            {
                for (var j = 0; j < coefficients.Length; ++j)
                {
                    result[i] += coefficients[j] * Math.Pow(nodes[i] - targetX, j);
                }
            }

            firstTaylors = new double[5];
            Array.Copy(result, firstTaylors, firstTaylors.Length);

            return result;
        }

        public double[] Adams(double[] firstValues = null, bool printDifferences = false)
        {
            if (firstTaylors == null && firstValues == null)
            {
                Taylor();
            }

            var first = firstValues ?? firstTaylors;

            var finiteDifferences = new double[6][];

            for (var i = 0; i < finiteDifferences.Length; ++i)
            {
                finiteDifferences[i] = new double[segmentNumber + 1];
            }

            for (var i = 0; i < first.Length; ++i)
            {
                finiteDifferences[0][i] = first[i];
                finiteDifferences[1][i] = segmentLength * derivative(nodes[i], finiteDifferences[0][i]);

            }

            for (var i = 0; i < finiteDifferences.Length - 2; ++i)
            {
                for (var j = i + 1; j < first.Length; ++j)
                {
                    finiteDifferences[i + 2][j] = finiteDifferences[i + 1][j] - finiteDifferences[i + 1][j - 1];
                }
            }

            // finiteDifferences.Format(Enumerable.Range(0, nodes.Length).Select(n => n.ToString()).ToArray(), new[]{ "y", "0", "1", "2", "3", "4" });

            for (var i = first.Length; i < finiteDifferences[0].Length; ++i)
            {
                var newYValue = finiteDifferences[0][i - 1] + finiteDifferences[1][i - 1] + finiteDifferences[2][i - 1] / 2
                    + 5 * finiteDifferences[3][i - 1] / 12 + 3 * finiteDifferences[4][i - 1] / 8 + 251 * finiteDifferences[5][i - 1] / 720;

                finiteDifferences[0][i] = newYValue;

                finiteDifferences[1][i] = segmentLength * derivative(nodes[i], finiteDifferences[0][i]);

                for (var j = 2; j < finiteDifferences.Length; ++j)
                {
                    finiteDifferences[j][i] = finiteDifferences[j - 1][i] - finiteDifferences[j - 1][i - 1];
                }

                // finiteDifferences.Format(Enumerable.Range(0, nodes.Length).Select(n => n.ToString()).ToArray(), new[]{ "y", "0", "1", "2", "3", "4" });
            }

            return finiteDifferences[0];
        }

        public double[] RungeKutta()
        {
            var result = new double[segmentNumber - 1];
            var rightNodes = nodes.TakeLast(segmentNumber - 1).ToArray();
            result[0] = targetY;

            for (var i = 1; i < result.Length; ++i)
            {
                var k1 = segmentLength * derivative(rightNodes[i - 1], result[i - 1]);
                var k2 = segmentLength * derivative(rightNodes[i - 1] + segmentLength * 0.5, result[i - 1] + k1 * 0.5);
                var k3 = segmentLength * derivative(rightNodes[i - 1] + segmentLength * 0.5, result[i - 1] + k2 * 0.5);
                var k4 = segmentLength * derivative(rightNodes[i], result[i - 1] + k3);

                result[i] = result[i - 1] + (k1 + 2 * k2 + 2 * k3 + k4) / 6.0;
            }

            return result;
        }

        public double[] Euler()
        {
            var result = new double[segmentNumber - 1];
            var rightNodes = nodes.TakeLast(segmentNumber - 1).ToArray();
            result[0] = targetY;

            for (var i = 1; i < result.Length; ++i)
            {
                result[i] = result[i - 1] + segmentLength * derivative(rightNodes[i - 1], result[i - 1]);
            }

            return result;
        }

        public double[] EulerI()
        {
            var result = new double[segmentNumber - 1];
            var rightNodes = nodes.TakeLast(segmentNumber - 1).ToArray();

            var halfSegment = 0.5 * segmentLength;

            result[0] = targetY;

            for (var i = 1; i < result.Length; ++i)
            {
                var middleValue = result[i - 1] + halfSegment * derivative(rightNodes[i - 1], result[i - 1]);
                result[i] = result[i - 1] + segmentLength * derivative(rightNodes[i - 1] + halfSegment, middleValue);
            }

            return result;
        }



        public double[] EulerII()
        {
            var result = new double[segmentNumber - 1];
            var rightNodes = nodes.TakeLast(segmentNumber - 1).ToArray();

            var halfSegment = 0.5 * segmentLength;

            result[0] = targetY;

            for (var i = 1; i < result.Length; ++i)
            {
                var previousDerivativeValue = derivative(rightNodes[i - 1], result[i - 1]);
                var preliminaryValue = result[i - 1] + segmentLength * previousDerivativeValue;
                result[i] = result[i - 1] + halfSegment * (derivative(rightNodes[i], preliminaryValue) + previousDerivativeValue);
            }

            return result;
        }
    }
}
