using System;
using System.Collections.Generic;
using System.Text;

namespace NonlinearEquations
{
    public static class RootFinder
    {
        private const double NewtonsMaxIterations = 30;
        private const double ModifiedNewtonsMaxIterations = 30;
        private const double SecantMaxIterations = 500;

        public static List<(double Start, double End)> Separate(Func<double, double> function, (double Start, double End) interval, int stepNumber)
        {
            // Check interval ends for being finite

            Console.WriteLine("* ОТДЕЛЕНИЕ КОРНЕЙ:\n" +
                $"* Интервал: [{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                $"* Число шагов: {stepNumber}\n");

            if (interval.Start >= interval.End)
            {
                throw new RootFinderException("Отделение корней", "Начало исходного интервала должно быть меньше конца.");
            }

            if (stepNumber < 2)
            {
                throw new RootFinderException("Отделение корней", "Число шагов должно быть больше 1.");
            }

            var result = new List<(double, double)>();

            var stepLength = (interval.End - interval.Start) / stepNumber;

            Console.WriteLine($"Рассчитанная длина шага: {stepLength.ToFormattedString()}\n");

            var x1 = interval.Start;
            var x2 = x1 + stepLength;

            var y1 = function(x1);
            var y2 = function(x2);

            Console.WriteLine($"x0: {x1.ToFormattedString()} | y0: {y1.ToFormattedString()}");

            for (var i = 1; i <= stepNumber; ++i)
            {
                // Check for zero separately.

                if (y1 * y2 <= 0)
                {
                    Console.WriteLine($"! Перемена знака");
                    result.Add((x1, x2));
                }

                Console.WriteLine($"x{i}: {x2.ToFormattedString()} | y{i}: {y2.ToFormattedString()}");

                if (i == stepNumber) break;

                x1 = x2;
                y1 = y2;

                x2 = (i == stepNumber - 1) ? interval.End : (x1 + stepLength);
                y2 = function(x2);
            }

            Console.WriteLine("\n* ОТДЕЛЕНИЕ КОРНЕЙ ЗАВЕРШЕНО:\n" +
                $"* Найдено промежутков перемены знака: {result.Count}\n" +
                $"-------------------------------------");

            return result;
        }

        public static (IntervalResult Result, double LastLength) Bisection(Func<double, double> function,
            (double Start, double End) interval, double epsilon)
        {
            Console.WriteLine("* БИСЕКЦИЯ:\n" +
                $"* Интервал: [{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                $"* Эпсилон: {epsilon.ToFormattedString()}\n");

            if (epsilon <= 0)
            {
                throw new RootFinderException("Бисекция", "Величина ε должна быть положительным числом.");
            }

            if (interval.Start >= interval.End)
            {
                throw new RootFinderException("Бисекция", "Начало исходного интервала должно быть меньше конца.");
            }

            if (function(interval.Start) * function(interval.End) >= 0)
            {
                throw new RootFinderException("Бисекция", "Значения функции на концах интервала должны иметь разные знаки.");
            }

            var start = interval.Start;
            var end = interval.End;
            var center = (start + end) / 2;
            var length = end - start;
            var yCenter = function(center);

            Console.WriteLine($"[{start.ToFormattedString()} >{center.ToFormattedString()}< {end.ToFormattedString()}]");

            var result = new IntervalResult { InitialApproximations = new List<double>() { center }, IterationCount = 1 };

            while (Math.Abs(yCenter) > epsilon && length > 2 * epsilon)
            {
                if (function(start) * yCenter <= 0)
                {
                    end = center;
                }
                else
                {
                    start = center;
                }

                center = (start + end) / 2;
                yCenter = function(center);
                length = end - start;

                Console.WriteLine($"[{start.ToFormattedString()} >{center.ToFormattedString()}< {end.ToFormattedString()}]");

                ++result.IterationCount;
            }

            result.AbsoluteResidual = Math.Abs(yCenter);
            result.Root = center;

            Console.WriteLine("\n* БИСЕКЦИЯ ЗАВЕРШЕНА:\n" +
                $"* Начальное приближение: {result.InitialApproximations[0].ToFormattedString()}\n" +
                $"* Итоговое приближение: {result.Root.ToFormattedString()}\n" +
                $"* Итераций: {result.IterationCount}\n" +
                $"* Абсолютная величина невязки: {result.AbsoluteResidual.ToFormattedString()}\n" +
                $"-------------------------------------");

            return (result, length / 2);
        }

        public static IntervalResult Newtons(Func<double, double> function, Func<double, double> derivative,
            Func<double, double> sDerivative, (double Start, double End) interval, double epsilon)
        {
            Console.WriteLine("* МЕТОД НЬЮТОНА:\n" +
                $"* Интервал: [{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                $"* Эпсилон: {epsilon.ToFormattedString()}\n");

            if (epsilon <= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Величина ε должна быть положительным числом.");
            }

            if (interval.Start >= interval.End)
            {
                throw new RootFinderException("Метод Ньютона", "Начало исходного интервала должно быть меньше конца.");
            }

            if (function(interval.Start) * function(interval.End) >= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Значения функции на концах интервала должны иметь разные знаки.");
            }

            var random = new Random();
            var startPoint = interval.Start + (interval.End - interval.Start) * random.NextDouble();
            var result = NewtonsWithStartPoint(function, derivative, sDerivative, interval, epsilon, startPoint);

            while (result == null)
            {
                startPoint = interval.Start + (interval.End - interval.Start) * random.NextDouble();
                result = NewtonsWithStartPoint(function, derivative, sDerivative, interval, epsilon, startPoint);
            }

            Console.WriteLine("\n* МЕТОД НЬЮТОНА: ВЫЧИСЛЕНИЕ ЗАВЕРШЕНО:\n" +
                $"* Начальное приближение: {result.InitialApproximations[0].ToFormattedString()}\n" +
                $"* Итоговое приближение: {result.Root.ToFormattedString()}\n" +
                $"* Итераций: {result.IterationCount}\n" +
                $"* Абсолютная величина невязки: {result.AbsoluteResidual.ToFormattedString()}\n" +
                $"-------------------------------------");

            return result;
        }

        private static IntervalResult NewtonsWithStartPoint(Func<double, double> function, Func<double, double> derivative,
            Func<double, double> sDerivative, (double Start, double End) interval, double epsilon, double startPoint)
        {
            Console.WriteLine($"Начальное приближение: {startPoint.ToFormattedString()}");

            if (startPoint <= interval.Start || startPoint >= interval.End) return null;

            if (function(startPoint) * sDerivative(startPoint) <= 0)
            {
                Console.WriteLine($"Начальное приближение не удовлетворяет условию f(x_0) * f''(x_0) > 0");
                return null;
            } 

            var result = new IntervalResult { IterationCount = 1, InitialApproximations = new List<double>() { startPoint } };

            try
            {
                var previous = startPoint;
                var current = previous - function(previous) / derivative(previous);

                while (Math.Abs(current - previous) > epsilon)
                {
                    if (result.IterationCount == NewtonsMaxIterations)
                    {
                        Console.WriteLine($"Достигнуто максимальное установленное число шагов");
                        return null;
                    } 

                    previous = current;
                    current = previous - function(previous) / derivative(previous);

                    ++result.IterationCount;
                }

                if (current <= interval.Start || current >= interval.End) return null;

                result.Root = current;
                result.AbsoluteResidual = Math.Abs(function(result.Root));
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine($"Деление на ноль");
                return null;
            }

            return result;
        }

        public static IntervalResult NewtonsModified(Func<double, double> function, Func<double, double> derivative,
            Func<double, double> sDerivative, (double Start, double End) interval, double epsilon)
        {
            Console.WriteLine("* МОДИФИЦИРОВАННЫЙ МЕТОД НЬЮТОНА:\n" +
                $"* Интервал: [{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                $"* Эпсилон: {epsilon.ToFormattedString()}\n");

            if (epsilon <= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Величина ε должна быть положительным числом.");
            }

            if (interval.Start >= interval.End)
            {
                throw new RootFinderException("Метод Ньютона", "Начало исходного интервала должно быть меньше конца.");
            }

            if (function(interval.Start) * function(interval.End) >= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Значения функции на концах интервала должны иметь разные знаки.");
            }

            var random = new Random();
            var startPoint = interval.Start + (interval.End - interval.Start) * random.NextDouble();
            var result = ModifiedNewtonsWithStartPoint(function, derivative, sDerivative, interval, epsilon, startPoint);

            while (result == null)
            {
                startPoint = interval.Start + (interval.End - interval.Start) * random.NextDouble();
                result = ModifiedNewtonsWithStartPoint(function, derivative, sDerivative, interval, epsilon, startPoint);
            }

            Console.WriteLine("\n* МОДИФИЦИРОВАННЫЙ МЕТОД НЬЮТОНА: ВЫЧИСЛЕНИЕ ЗАВЕРШЕНО:\n" +
                $"* Начальное приближение: {result.InitialApproximations[0].ToFormattedString()}\n" +
                $"* Итоговое приближение: {result.Root.ToFormattedString()}\n" +
                $"* Итераций: {result.IterationCount}\n" +
                $"* Абсолютная величина невязки: {result.AbsoluteResidual.ToFormattedString()}\n" +
                $"-------------------------------------");

            return result;
        }

        private static IntervalResult ModifiedNewtonsWithStartPoint(Func<double, double> function, Func<double, double> derivative,
            Func<double, double> sDerivative, (double Start, double End) interval, double epsilon, double startPoint)
        {
            Console.WriteLine($"Начальное приближение: {startPoint.ToFormattedString()}");

            if (startPoint <= interval.Start || startPoint >= interval.End) return null;

            if (function(startPoint) * sDerivative(startPoint) <= 0)
            {
                Console.WriteLine($"Начальное приближение не удовлетворяет условию f(x_0) * f''(x_0) > 0");
                return null;
            } 

            var startPointDerivative = derivative(startPoint);

            if (startPointDerivative == 0)
            {
                Console.WriteLine($"Производная в точке начального приближения равна нулю");
                return null;
            } 

            var result = new IntervalResult { IterationCount = 1, InitialApproximations = new List<double>() { startPoint } };

            var previous = startPoint;
            var current = previous - function(previous) / startPointDerivative;

            while (Math.Abs(current - previous) > epsilon)
            {
                if (result.IterationCount == ModifiedNewtonsMaxIterations)
                {
                    Console.WriteLine($"Достигнуто максимальное установленное число шагов");
                    return null;
                } 

                previous = current;
                current = previous - function(previous) / startPointDerivative;

                ++result.IterationCount;
            }

            if (current <= interval.Start || current >= interval.End) return null;

            result.Root = current;
            result.AbsoluteResidual = Math.Abs(function(result.Root));

            return result;
        }

        public static IntervalResult Secant(Func<double, double> function,
            (double Start, double End) interval, double epsilon)
        {
            Console.WriteLine("* МЕТОД СЕКУЩИХ:\n" +
                $"* Интервал: [{interval.Start.ToFormattedString()}; {interval.End.ToFormattedString()}]\n" +
                $"* Эпсилон: {epsilon.ToFormattedString()}\n");

            if (epsilon <= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Величина ε должна быть положительным числом.");
            }

            if (interval.Start >= interval.End)
            {
                throw new RootFinderException("Метод Ньютона", "Начало исходного интервала должно быть меньше конца.");
            }

            if (function(interval.Start) * function(interval.End) >= 0)
            {
                throw new RootFinderException("Метод Ньютона", "Значения функции на концах интервала должны иметь разные знаки.");
            }

            var random = new Random();
            var startPoint1 = interval.Start + (interval.End - interval.Start) * random.NextDouble();
            var startPoint2 = interval.Start + (interval.End - interval.Start) * random.NextDouble();
            var result = SecantWithStartPoints(function, interval, epsilon, startPoint1, startPoint2);

            while (result == null)
            {
                startPoint1 = interval.Start + (interval.End - interval.Start) * random.NextDouble();
                startPoint2 = interval.Start + (interval.End - interval.Start) * random.NextDouble();
                result = SecantWithStartPoints(function, interval, epsilon, startPoint1, startPoint2);
            }

            Console.WriteLine("\n* МЕТОД СЕКУЩИХ: ВЫЧИСЛЕНИЕ ЗАВЕРШЕНО:\n" +
                $"* Начальное приближение: {result.InitialApproximations[0].ToFormattedString()}\n" +
                $"* Итоговое приближение: {result.Root.ToFormattedString()}\n" +
                $"* Итераций: {result.IterationCount}\n" +
                $"* Абсолютная величина невязки: {result.AbsoluteResidual.ToFormattedString()}\n" +
                $"-------------------------------------");

            return result;
        }

        private static IntervalResult SecantWithStartPoints(Func<double, double> function,
            (double Start, double End) interval, double epsilon, double startPoint1, double startPoint2)
        {
            Console.WriteLine($"Начальные приближения: {startPoint1.ToFormattedString()}; {startPoint2.ToFormattedString()}");

            if (startPoint1 == startPoint2) return null;

            if (startPoint1 <= interval.Start || startPoint1 >= interval.End) return null;

            if (startPoint2 <= interval.Start || startPoint2 >= interval.End) return null;

            var result = new IntervalResult { IterationCount = 1,
                InitialApproximations = new List<double>() { startPoint1, startPoint2 } };

            var first = startPoint1;
            var second = startPoint2;
            var ySecond = function(second);
            var third = second - ySecond / (ySecond - function(first)) * (second - first);

            while (Math.Abs(third - second) > epsilon)
            {
                if (result.IterationCount == SecantMaxIterations)
                {
                    Console.WriteLine($"Достигнуто максимальное установленное число шагов");
                    return null;
                }

                first = second;
                second = third;
                ySecond = function(second);
                third = second - ySecond / (ySecond - function(first)) * (second - first);

                ++result.IterationCount;
            }

            if (third <= interval.Start || third >= interval.End) 
            {
                Console.WriteLine($"Найденное приближение лежит вне интервала");
                return null;
            } 

            result.Root = third;
            result.AbsoluteResidual = Math.Abs(function(result.Root));

            return result;
        }
    }
}
