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
            return Math.Cos(x) + 2;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

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

            Console.Write("Введите начало отрезка: ");

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

            Console.WriteLine($"Таблица значений функции (число значений: {numberOfValues})");
            Console.Write(functionTable.Format());

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

            Console.WriteLine($"Узлы интерполяции");
            Console.Write(dataPoints.Format());

            Console.WriteLine($"Точка интерполирования: {interpolationPoint.Format()}\n" +
                $"Степень интерполяционного многочлена: {interpolationDegree}\n");


            var interpolated = new NewtonsPolynomial(dataPoints);
            var interpolationPointY = interpolated.ToFunction()(interpolationPoint);

            Console.WriteLine($"Значение интерполяционного многочлена в точке {interpolationPoint.Format()}: " +
                $"{interpolationPointY.Format()}");

            Console.WriteLine($"Абсолютная фактическая погрешность: " +
                $"{Math.Abs(Function(interpolationPoint) - interpolationPointY).Format()}");

            if (Console.ReadLine() == "q")
            {
                return false;
            }

            return true;
        }
    }
}
