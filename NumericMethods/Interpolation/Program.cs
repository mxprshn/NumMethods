using NumAn;
using NumAn.Interpolation;
using System;
using System.Linq;

namespace Interpolation
{
    class Program
    {
        static double Function(double x)
        {
            // return 2 * Math.Pow(x, 2) + 10 * Math.Pow(x, 4);
            return Math.Cos(x) + 2;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #2\n\n" +
                "Задача алгебраического интерполирования\n" +
                "Интерполяционные многочлены Ньютона и Лагранжа\n\n" +
                "Вариант #10\n");

            Console.Write("Введите число значений в таблице функции: ");

            var numberOfValues = 0;

            while (!int.TryParse(Console.ReadLine(), out numberOfValues) || numberOfValues < 2)
            {
                Console.Write("Некорректное значение: введите целое число, большее единицы: ");
            }

            Console.Write("Введите начало отрезка: ");

            var start = 0.0;

            while (!double.TryParse(Console.ReadLine(), out start))
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write("Введите конец отрезка: ");

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

            while (Interpolation(functionTable)) { };
        }

        private static bool Interpolation((double x, double y)[] functionTable)
        {
            Console.Write("Введите точку интерполяции: ");

            var interpolationPoint = 0.0;

            while (!double.TryParse(Console.ReadLine(), out interpolationPoint))
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write($"Введите степень интерполяционного многочлена (< {functionTable.Length}): ");

            var interpolationDegree = 0;

            while (!int.TryParse(Console.ReadLine(), out interpolationDegree) ||
                interpolationDegree < 1 || interpolationDegree >= functionTable.Length)
            {
                Console.Write("Некорректное значение -- введите целое число, большее нуля и меньшее числа значений в " +
                              "таблице функции: ");
            }

            var dataPoints = functionTable.OrderBy(value => Math.Abs(interpolationPoint - value.x))
                .Take(interpolationDegree + 1).ToArray();

            Console.WriteLine($"\nУзлы интерполяции");
            Console.WriteLine(dataPoints.Format("x", "f(x)"));

            Console.WriteLine($"Точка интерполирования: {interpolationPoint.Format()}\n" +
                $"Степень интерполяционного многочлена: {interpolationDegree}\n");


            var newtonInterpolated = new NewtonsPolynomial(dataPoints);
            var newtonInterpolationPointY = newtonInterpolated.ToFunction()(interpolationPoint);

            var lagrangeInterpolated = new LagrangePolynomial(dataPoints);
            var lagrangeInterpolationPointY = lagrangeInterpolated.ToFunction()(interpolationPoint);

            Console.WriteLine($"Значение интерполяционного многочлена Ньютона в точке {interpolationPoint.Format()}: " +
                $"{newtonInterpolationPointY.Format()}");

            Console.WriteLine($"Абсолютная фактическая погрешность: " +
                $"{Math.Abs(Function(interpolationPoint) - newtonInterpolationPointY).Format()}");

            Console.WriteLine($"Значение интерполяционного многочлена Лагранжа в точке {interpolationPoint.Format()}: " +
                $"{lagrangeInterpolationPointY.Format()}");

            var originalValue = Function(interpolationPoint);

            Console.WriteLine($"Абсолютная фактическая погрешность: " +
                $"{Math.Abs(originalValue - lagrangeInterpolationPointY).Format()}");

            Console.WriteLine($"Значение исходной функции в точке интерполяции: " +
                $"{originalValue.Format()}");

            Console.WriteLine($"Модуль разности значений интерполяционных многочленов Ньютона и Лагранжа: " +
                $"{Math.Abs(newtonInterpolationPointY - lagrangeInterpolationPointY).Format()}");

            Console.WriteLine($"\nЧтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}
