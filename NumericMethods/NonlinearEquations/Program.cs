using System;

namespace NonlinearEquations
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<double, double> function = (x => 1.2 * Math.Pow(x, 4) + 2 * Math.Pow(x, 3) - 13 * Math.Pow(x, 2) - 14.2 * x - 24.1);
            Func<double, double> derivative = (x =>
                4.8 * Math.Pow(x, 3) + 6 * Math.Pow(x, 2) - 26 * x - 14.2);

            Func<double, double> sDerivative = (x =>
                14.4 * Math.Pow(x, 2) + 12 * x - 26);
            var results = RootFinder.Separate(function, (-5, 5), 177);

            foreach (var result in results)
            {
                RootFinder.Secant(function, result, 0.000001);
                RootFinder.Newtons(function, derivative, sDerivative, result, 0.000001);
                RootFinder.NewtonsModified(function, derivative, sDerivative, result, 0.000001);
                RootFinder.Bisection(function, result, 0.000001);
            }
            
        }
    }
}
