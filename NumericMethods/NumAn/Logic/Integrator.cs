using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public static class Integrator
    {


        public class Integral
        {
            public Integral(double leftRecangle, double rightRecangle, double middleRectangle, double trapezoidal, double simpsons)
            {
                LeftRecangle = leftRecangle;
                RightRecangle = rightRecangle;
                MiddleRecangle = middleRectangle;
                Trapezoidal = trapezoidal;
                Simpsons = simpsons;
            }

            public double LeftRecangle { get; private set; }
            public double RightRecangle { get; private set; }
            public double MiddleRecangle { get; private set; }
            public double Trapezoidal { get; private set; }
            public double Simpsons { get; private set; }
        }

        public static Integral Integrate(Func<double, double> function, double left, double right, int segmentNumber)
        {
            if (left >= right || segmentNumber < 1 || !left.IsNumber() || !right.IsNumber())
            {
                throw new ArgumentException();
            }
            
            var firstValue = 0d;
            var lastValue = 0d;
            var trimmedSum = 0d;
            var middleSum = 0d;

            var segmentLength = (right - left) / segmentNumber;

            for (var i = 0; i < segmentNumber; ++i)
            {
                var value = function(left + segmentLength * i);
                var middleValue = function(left + segmentLength * i + segmentLength / 2);

                if (i == 0) 
                {
                    firstValue = value;
                }
                else
                {
                    trimmedSum += value;
                }

                middleSum += middleValue;
            }

            lastValue = function(right);

            var leftRectangle = segmentLength * (firstValue + trimmedSum);
            var rightRectangle = segmentLength * (lastValue + trimmedSum);
            var middleRectangle = segmentLength * middleSum;
            var trapezoidal = segmentLength * (0.5d * (firstValue + lastValue) + trimmedSum);
            var simpsons = segmentLength / 6 * (4 * middleSum + 2 * trimmedSum + firstValue + lastValue);

            return new Integral(leftRectangle, rightRectangle, middleRectangle, trapezoidal, simpsons);
        }

        public static double GaussIntegrate(Func<double, double> function, double left, double right, int segmentNumber)
        {
            var segmentLength = (right - left) / segmentNumber;
            var halfSegmentLength = segmentLength * 0.5;            
            var shift = halfSegmentLength / Math.Sqrt(3);

            var sum = 0d;
            var center = halfSegmentLength;

            for (var i = 0; i < segmentNumber; ++i)
            {
                sum += function(center - shift) + function(center + shift);

                center += segmentLength;
            }

            return sum * halfSegmentLength;
        }

        public static double GaussLikeIntegrate(Func<double, double> function, Func<double, double> weight, double left, double right, ILogger logger = null)
        {
            logger.Log("Построение КФ типа Гаусса...");

            const int WeightIntegrationSegmentNumber = 1000000;

            var momentum0 = Integrate(weight, left, right, WeightIntegrationSegmentNumber).MiddleRecangle;
            var momentum1 = Integrate(x => weight(x) * x, left, right, WeightIntegrationSegmentNumber).MiddleRecangle;
            var momentum2 = Integrate(x => weight(x) * Math.Pow(x, 2), left, right, WeightIntegrationSegmentNumber).MiddleRecangle;
            var momentum3 = Integrate(x => weight(x) * Math.Pow(x, 3), left, right, WeightIntegrationSegmentNumber).MiddleRecangle;

            logger?.Log($"Момент 0: {momentum0.Format(5)}");
            logger?.Log($"Момент 1: {momentum1.Format(5)}");
            logger?.Log($"Момент 2: {momentum2.Format(5)}");
            logger?.Log($"Момент 3: {momentum3.Format(5)}");

            var quadCoefficient1 = (momentum0 * momentum3 - momentum2 * momentum1) / (momentum1 * momentum1 - momentum2 * momentum0);
            var quadCoefficient2 = (momentum2 * momentum2 - momentum3 * momentum1) / (momentum1 * momentum1 - momentum2 * momentum0);

            logger?.Log($"Ортогональный многочлен: x^2 + {quadCoefficient1.Format(5)} * x + {quadCoefficient2}");

            var discriminantRoot = Math.Sqrt(quadCoefficient1 * quadCoefficient1 - 4 * quadCoefficient2);

            var node1 = (-quadCoefficient1 + discriminantRoot) / 2;
            var node2 = (-quadCoefficient1 - discriminantRoot) / 2;

            logger?.Log($"Узел 1: {node1.Format(5)}");
            logger?.Log($"Узел 2: {node2.Format(5)}");

            var formulaCoefficient1 = (momentum1 - node2 * momentum0) / (node1 - node2);
            var formulaCoefficient2 = (momentum1 - node1 * momentum0) / (node2 - node1);

            logger?.Log($"Коэффициент КФ 1: {formulaCoefficient1.Format(5)}");
            logger?.Log($"Коэффициент КФ 2: {formulaCoefficient2.Format(5)}");
            logger?.Log($"Модуль разности суммы коэффициентов и нулевого момента: {Math.Abs(formulaCoefficient1 + formulaCoefficient2 - momentum0).Format(10)}");

            return formulaCoefficient1 * function(node1) + formulaCoefficient2 * function(node2);
        }

        public static double MelerIntegrate(Func<double, double> function, int nodeNumber)
        {
            var sum = 0d;

            for (var i = 1; i <= nodeNumber; ++i)
            {
                sum += function(Math.Cos(Math.PI * (2 * i - 1) / (2 * nodeNumber)));
            }

            return sum * Math.PI / nodeNumber;
        }
    }
}