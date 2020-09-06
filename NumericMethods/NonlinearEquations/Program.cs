using System;

namespace NonlinearEquations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Func<double, double> function = (x =>
                1.2 * Math.Pow(x, 4) + 2 * Math.Pow(x, 3) - 13 * Math.Pow(x, 2) - 14.2 * x - 24.1);

            Func<double, double> derivative = (x =>
                4.8 * Math.Pow(x, 3) + 6 * Math.Pow(x, 2) - 26 * x - 14.2);

            Func<double, double> sDerivative = (x =>
                14.4 * Math.Pow(x, 2) + 12 * x - 26);

            double epsilon = 0.000001;

            (double Start, double End) interval = (-5, 5);

            Console.WriteLine("** ЛАБОРАТОРНАЯ РАБОТА №1\n" +
                "** ЧИСЛЕННЫЕ МЕТОДЫ РЕШЕНИЯ НЕЛИНЕЙНЫХ УРАВНЕНИЙ\n" +
                $"** Интервал:[{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                "** Функция: f(x) = 1.2x^4 + 2x^3 - 13x^2 - 14.2x - 24.1\n" +
                $"** Эпсилон: {epsilon.ToFormattedString()}\n" +
                "-------------------------------------");

            try
            {
                var results = RootFinder.Separate(function, interval);

                foreach (var result in results)
                {
                    if (result.Start == 0 || result.End == 0)
                    {
                        Console.WriteLine($"Один из концов промежутка [{result.Start.ToFormattedString()}; {result.End.ToFormattedString()}]\n" +
                            "является корнем уравнения -- к данному промежутку методы не будут применяться.\n" +
                            "-------------------------------------");
                    }
                }

                foreach (var result in results)
                {
                    RootFinder.Bisection(function, result, epsilon);
                }

                foreach (var result in results)
                {
                    RootFinder.Newtons(function, derivative, sDerivative, result, epsilon);
                }

                foreach (var result in results)
                {
                    RootFinder.NewtonsModified(function, derivative, sDerivative, result, epsilon);
                }

                foreach (var result in results)
                {
                    RootFinder.Secant(function, result, epsilon);
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"ОШИБКА: Некорректный параметр: {e.Message}");
            }
        }
    }
}
