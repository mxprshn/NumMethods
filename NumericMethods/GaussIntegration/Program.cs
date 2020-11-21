using System;
using NumAn;
using MathNet.Numerics.Integration;

namespace GaussIntegration
{
    class Program
    {
        private class Logger : ILogger
        {
            public void Log(string message)
            {
                Console.WriteLine(message);
            }
        }

        private const int MathNetIntegrationNodeNumber = 1024;
        private static readonly double MelerExpectedIntegral = Math.PI / Math.Sqrt(2);

        private static double FunctionForGauss(double x)
        {
            //return Math.Sin(x);
            return 1;
            // return x;
            // return Math.Pow(x, 2);
            // return Math.Pow(x, 3);
        }

        private static double WeightForGauss(double x)
        {
            return 1 / (x + 0.1);
        }

        private static double FunctionForMeler(double x)
        {
            return 1 / (1 + Math.Pow(x, 2));
        }

        private static double WeightForMeler(double x)
        {
            return 1 / Math.Sqrt(1 - Math.Pow(x, 2));
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #5\n\n" +
                "Приближённое вычисление интегралов при помощи квадратурных формул Наивысшей Алгебраической Степени Точности\n\n" +
                "Вариант #10\n\n");

            while (Integration()) { };
        }

        private static bool Integration()
        {
            Console.Write("Введите левый конец отрезка интегрирования по составной КФ Гаусса и КФ типа Гаусса: ");

            var left = 0d;

            while (!double.TryParse(Console.ReadLine(), out left) || !left.IsNumber())
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write("Введите правый конец отрезка интегрирования по составной КФ Гаусса и КФ типа Гаусса: ");

            var right = 0d;

            while (!double.TryParse(Console.ReadLine(), out right) || !right.IsNumber() || right <= left)
            {
                Console.Write("Некорректное значение: введите вещественное число, большее левого конца отрезка интегрирования: ");
            }

            Console.Write("Введите число промежутков деления отрезка интегрирования по составной КФ Гаусса: ");

            var segmentNumber = 0;

            while (!int.TryParse(Console.ReadLine(), out segmentNumber) || segmentNumber <= 0)
            {
                Console.Write("Некорректное значение: введите положительное целое число: ");
            }

            Console.Write("Введите число узлов для интегрирования по КФ Мелера: ");

            var nodeNumber = 0;

            while (!int.TryParse(Console.ReadLine(), out nodeNumber) || nodeNumber <= 0)
            {
                Console.Write("Некорректное значение: введите положительное целое число: ");
            }

            FormatExtensions.PrintDivider();

            var segmentLength = (right - left) / segmentNumber;

            var gaussIntegral = Integrator.GaussIntegrate(x => FunctionForGauss(x) * WeightForGauss(x), left, right, segmentNumber);
            var gaussLikeIntegral = Integrator.GaussLikeIntegrate(FunctionForGauss, WeightForGauss, left, right, new Logger());
            var melerIntegral = Integrator.MelerIntegrate(FunctionForMeler, nodeNumber);

            var gaussExpectedIntegral = GaussLegendreRule.Integrate(x => FunctionForGauss(x) * WeightForGauss(x), left, right, MathNetIntegrationNodeNumber);
            FormatExtensions.PrintDivider();

            Console.WriteLine($"Функция: f(x) = sin(x) | Вес: 1 / (x + 0.1)");
            Console.WriteLine($"Отрезок интегрирования: [{left.Format(5)}, {right.Format(5)}]");
            Console.WriteLine($"Фактическое значение интеграла: {gaussExpectedIntegral.Format()}");
            Console.WriteLine($"Число отрезков деления для КФ Гаусса: {segmentNumber}");
            Console.WriteLine($"Длина отрезка для КФ Гаусса: {segmentLength.Format()}\n");

            Console.WriteLine(
                $"Формула Гаусса\n" +
                $"Вычисленное приближенное значение: {gaussIntegral.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(gaussExpectedIntegral - gaussIntegral).Format()}\n\n" +
                $"Формула типа Гаусса\n" +
                $"Вычисленное приближенное значение: {gaussLikeIntegral.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(gaussExpectedIntegral - gaussLikeIntegral).Format()}\n\n");

            FormatExtensions.PrintDivider();

            Console.WriteLine($"Функция: f(x) = 1 / (1 + x^2) | Вес: 1 / sqrt(1 - x^2)");
            Console.WriteLine($"Отрезок интегрирования: [-1, 1]");
            Console.WriteLine($"Фактическое значение интеграла: {MelerExpectedIntegral.Format()}");
            Console.WriteLine($"Число узлов для КФ Мелера: {nodeNumber}\n");

            Console.WriteLine(
                $"Формула Мелера\n" +
                $"Вычисленное приближенное значение: {melerIntegral.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(MelerExpectedIntegral - melerIntegral).Format()}\n\n");

            Console.WriteLine($"Чтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}
