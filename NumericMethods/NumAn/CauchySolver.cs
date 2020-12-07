using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public class CauchySolver
    {
        private double[] nodes;
        private double[] firstTaylors;
        private double[] derivatives;

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

            this.segmentLength = segmentLength;
            this.segmentNumber = segmentNumber;
            this.targetX = targetX;
            this.targetY = targetY;

            nodes = new double[segmentNumber + 1];

            for (var i = -2; i <= segmentNumber - 2; ++i)
            {
                nodes[i + 2] = targetX + i * segmentNumber;
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

        public List<(double x, double y)> Adams()
        {
            if (firstTaylors == null)
            {
                Taylor();
            }

            var finiteDifferences = new double[6][];
            for (var i = 0; i < finiteDifferences.Length; ++i)
            {
                finiteDifferences[i] = new double[segmentNumber + 1];
            }

            Array.Copy(firstTaylors, finiteDifferences[0], firstTaylors.Length);

            for (var i = 0; i < finiteDifferences.Length - 1; ++i)
            {
                for (var j = 0; j < firstTaylors.Length - i; ++j)
                {
                    finiteDifferences[j] = finiteDifferences[]
                }
            }

        }

        public List<(double x, double y)> RungeKutta()
        {

        }

        public List<(double x, double y)> Euler(double segmentLength, int segmentNumber)
        {

        }

        public List<(double x, double y)> EulerI(double segmentLength, int segmentNumber)
        {

        }



        public List<(double x, double y)> EulerII(double segmentLength, int segmentNumber)
        {

        }
    }
}
