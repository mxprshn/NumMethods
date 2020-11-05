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
    }
}