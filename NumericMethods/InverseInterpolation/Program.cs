using System;
using System.Linq;
using NumAn;
using NumAn.Interpolation;
using NumAn.RootFinding;

namespace InverseInterpolation
{
    class Program
    {
        static double Function(double x)
        {
            // return 2 * Math.Pow(x, 2) + 10 * Math.Pow(x, 4);
            // return 2 * x;
            return Math.Exp(x) + 1;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #3.1\n\n" +
                "Задача обратного интерполирования\n\n" +
                "Вариант #10\n");

            Console.Write("Введите число значений в таблице функции (m + 1): ");

            var numberOfValues = 0;

            while (!int.TryParse(Console.ReadLine(), out numberOfValues) || numberOfValues < 2)
            {
                Console.Write("Некорректное значение: введите целое число, большее единицы: ");
            }

            Console.Write("Введите начало отрезка (a): ");

            var start = 0.0;

            while (!double.TryParse(Console.ReadLine(), out start))
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write("Введите конец отрезка (b): ");

            var end = 0.0;

            while (!double.TryParse(Console.ReadLine(), out end) || end <= start)
            {
                Console.Write("Некорректное значение: введите вещественное число, большее чем начало отрезка: ");
            }

            var segmentLength = (end - start) / (numberOfValues - 1);

            var functionTable = new (double x, double y)[numberOfValues];

            for (var i = 0; i < numberOfValues; ++i)
            {
                var x = start + segmentLength * i;
                functionTable[i] = (x, Function(x));
            }

            Console.WriteLine($"\nТаблица значений функции (число значений: {numberOfValues})");
            Console.WriteLine(functionTable.Format("x", "f(x)"));

            while (InverseInterpolation(functionTable)) { };
        }

        private static bool InverseInterpolation((double x, double y)[] functionTable)
        {
            Console.Write("Введите значение функции в точке интерполяции (F): ");

            var interpolationPointY = 0.0;

            while (!double.TryParse(Console.ReadLine(), out interpolationPointY))
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write($"Введите степень интерполяционного многочлена для f^-1 (< {functionTable.Length}): ");

            var inverseInterpolationDegree = 0;

            while (!int.TryParse(Console.ReadLine(), out inverseInterpolationDegree) ||
                inverseInterpolationDegree < 1 || inverseInterpolationDegree >= functionTable.Length)
            {
                Console.Write("Некорректное значение -- введите целое число, большее нуля и меньшее числа значений в " +
                              "таблице функции: ");
            }

            Console.Write($"Введите степень интерполяционного многочлена для f (< {functionTable.Length}): ");

            var interpolationDegree = 0;

            while (!int.TryParse(Console.ReadLine(), out interpolationDegree) ||
                interpolationDegree < 1 || interpolationDegree >= functionTable.Length)
            {
                Console.Write("Некорректное значение -- введите целое число, большее нуля и меньшее числа значений в " +
                              "таблице функции: ");
            }

            Console.Write("Введите точность (epsilon) решения уравнения: ");

            var precision = 0.0;

            while (!double.TryParse(Console.ReadLine(), out precision) || precision <= 0.0)
            {
                Console.Write("Некорректное значение: введите положительное вещественное число: ");
            }

            var inverseDataPoints = functionTable.Select(value => (value.y, value.x))
                .OrderBy(value => Math.Abs(interpolationPointY - value.Item1))
                .Take(inverseInterpolationDegree + 1).ToArray();

            Console.WriteLine($"\nУзлы интерполяции для f^-1(x)");
            Console.WriteLine(inverseDataPoints.Format("x", "f^-1(x)"));

            var ordered = functionTable.OrderBy(value => value.x).ToArray();
            var approximateInterpolationPoint = 0.0;

            if (interpolationPointY < ordered[0].y)
            {
                approximateInterpolationPoint = ordered[0].x;
            }
            else if (interpolationPointY > ordered.Last().y)
            {
                approximateInterpolationPoint = ordered.Last().x;
            }
            else
            {
                for (var i = 0; i < ordered.Length - 1; ++i)
                {
                    if (ordered[i].y == interpolationPointY)
                    {
                        approximateInterpolationPoint = ordered[i].x;
                        break;
                    }

                    if (ordered[i + 1].y == interpolationPointY)
                    {
                        approximateInterpolationPoint = ordered[i + 1].x;
                        break;
                    }

                    if (ordered[i].y < interpolationPointY && interpolationPointY < ordered[i + 1].y)
                    {
                        approximateInterpolationPoint = ordered[i].x + (ordered[i + 1].x - ordered[i].x) / 2;
                        break;
                    }
                }
            }

            var dataPoints = functionTable.OrderBy(value => Math.Abs(approximateInterpolationPoint - value.x))
                .Take(interpolationDegree + 1).ToArray();

            Console.WriteLine($"\nУзлы интерполяции для f(x)");
            Console.WriteLine(dataPoints.Format("x", "f(x)"));            

            Console.WriteLine($"Значение функции в точке интерполирования (F): {interpolationPointY.Format()}\n" +
                $"Ближайшая точка x для узлов интерполяции f: {approximateInterpolationPoint}\n" +
                $"Степень интерполяционного многочлена для f^-1: {inverseInterpolationDegree}\n" +
                $"Степень интерполяционного многочлена для f: {interpolationDegree}\n");

            var inversePolynomial = new NewtonsPolynomial(inverseDataPoints);
            var inversePolynomialY = inversePolynomial.ToFunction()(interpolationPointY);

            Console.WriteLine($"Значение x, полученное при интерполировании обратной функции: " +
                $"{inversePolynomialY.Format()}");

            Console.WriteLine($"Модуль невязки: " +
                $"{Math.Abs(Function(inversePolynomialY) - interpolationPointY).Format()}\n");

            var polynomial = new NewtonsPolynomial(dataPoints);
            var polynomialFunction = polynomial.ToFunction();

            Func<double, double> targetFunction = x => polynomialFunction(x) - interpolationPointY;

            var separatedIntervals = RootSeparation.Separate(targetFunction, (ordered[0].x, ordered.Last().x));

            if (separatedIntervals.Count == 0)
            {
                Console.WriteLine("Интервалов перемены знака не найдено.");
            }
            else
            {
                var rootFinder = new BisectionRootFinder(targetFunction);

                foreach (var interval in separatedIntervals)
                {
                    var result = rootFinder.FindRoots(interval, precision);

                    Console.WriteLine($"Значение x, полученное при решении уравнения: " +
                    $"{result.Root.Format()}");

                    Console.WriteLine($"Модуль невязки: " +
                        $"{Math.Abs(Function(result.Root) - interpolationPointY).Format()}");
                }
            }

            Console.WriteLine($"\nЧтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}