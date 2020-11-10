using System;
using NumAn;
using NumSharp;
using MathNet;
using CenterSpace.NMath.Analysis;

namespace Integration
{
    class Program
    {
        private static double Function(double x)
        {
            return Math.Exp(x) + x;
            // return 73;
            // return 5 * x;
            // return 5 * Math.Pow(x, 3) + 4 * Math.Pow(x, 2) + 1.0;
            // return Math.Sin(x);
        }

        private static double Antiderivative(double x)
        {
            return Math.Exp(x) + Math.Pow(x, 2) / 2.0;
            // return 73 * x;
            // return 5 * Math.Pow(x, 2) / 2.0;
            // return 5 * Math.Pow(x, 4) / 4.0 + 4 * Math.Pow(x, 3) / 3.0 + x;
            // return -Math.Cos(x);
        }

        private static Func<double, double> Derivative(int order)
        {
            switch (order)
            {
                case 1: return x => Math.Exp(x) + 1;
                case 2: return x => Math.Exp(x);
                case 3: return x => Math.Exp(x);
                case 4: return x => Math.Exp(x);
            }

            // return x => 0.0;

            //switch (order)
            //{
            //    case 1: return x => 5;
            //    default: return x => 0;
            //}

            //switch (order)
            //{
            //    case 1: return x => x * (15 * x + 8);
            //    case 2: return x => 30 * x + 8;
            //    case 3: return x => 30;
            //    case 4: return x => 0;
            //}

            //switch (order)
            //{
            //    case 1: return x => Math.Cos(x);
            //    case 2: return x => - Math.Sin(x);
            //    case 3: return x => - Math.Cos(x);
            //    case 4: return x => Math.Sin(x);
            //}

            throw new ArgumentException();
        }

        private static double MonotonousFunctionAbsMax(Func<double, double> function, double left, double right) =>
            Math.Max(Math.Abs(function(right)), Math.Abs(function(left)));

        private static double Weight(double x) => 1;

        private const double LeftConst = 0.5;
        private const double RightConst = 0.5;
        private const double MiddleConst = 1.0d / 24.0d;
        private const double TrapezoidalConst = 1.0d / 12.0d;
        private const double SimpsonsConst = 1.0d / 2880.0d;

        private const int LeftDegree = 0;
        private const int RightDegree = 0;
        private const int MiddleDegree = 1;
        private const int TrapezoidalDegree = 1;
        private const int SimpsonsDegree = 3;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #4\n\n" +
                "Приближённое вычисление интеграла по составным квадратурным формулам\n\n");

            while (Integration()) { };
        }

        private static bool Integration()
        {
            Console.Write("Введите левый конец отрезка интегрирования: ");

            var left = 0d;

            while (!double.TryParse(Console.ReadLine(), out left) || !left.IsNumber())
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write("Введите правый конец отрезка интегрирования: ");

            var right = 0d;

            while (!double.TryParse(Console.ReadLine(), out right) || !right.IsNumber() || right <= left)
            {
                Console.Write("Некорректное значение: введите вещественное число, большее левого конца отрезка интегрирования: ");
            }

            Console.Write("Введите число промежутков деления отрезка интегрирования: ");

            var segmentNumber = 0;

            while (!int.TryParse(Console.ReadLine(), out segmentNumber) || segmentNumber <= 0)
            {
                Console.Write("Некорректное значение: введите положительное целое число: ");
            }

            var segmentLength = (right - left) / segmentNumber;

            Func<double, double> composition = (x => Weight(x) * Function(x));

            var integral = Integrator.Integrate(composition, left, right, segmentNumber);

            var precise = (Antiderivative(right) - Antiderivative(left));

            Func<double, int, double> theoreticalError = (c, d) =>
            {
                return (right - left) * MonotonousFunctionAbsMax(Derivative(d), left, right) * c * Math.Pow(segmentLength, d);
            };

            Console.WriteLine($"\nФункция: f(x) = exp(x) + x");
            Console.WriteLine($"Отрезок интегрирования: [{left.Format(5)}, {right.Format(5)}]");
            Console.WriteLine($"Число отрезков деления: {segmentNumber}");
            Console.WriteLine($"Длина отрезка: {segmentLength.Format()}\n");

            Console.WriteLine($"Точное значение интеграла: {precise.Format()}\n");

            Console.WriteLine(
                $"Формула левых прямоугольников\n" +
                $"Вычисленное приближенное значение: {integral.LeftRecangle.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(integral.LeftRecangle - precise).Format()}\n" +
                $"Теоретическая погрешность: {theoreticalError(LeftConst, LeftDegree + 1).Format()}\n\n" +
                $"Формула правых прямоугольников\n" +
                $"Вычисленное приближенное значение: {integral.RightRecangle.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(integral.RightRecangle - precise).Format()}\n" +
                $"Теоретическая погрешность: {theoreticalError(RightConst, RightDegree + 1).Format()}\n\n" +
                $"Формула средних прямоугольников\n" +
                $"Вычисленное приближенное значение: {integral.MiddleRecangle.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(integral.MiddleRecangle - precise).Format()}\n" +
                $"Теоретическая погрешность: {theoreticalError(MiddleConst, MiddleDegree + 1).Format()}\n\n" +
                $"Формула трапеций\n" +
                $"Вычисленное приближенное значение: {integral.Trapezoidal.Format()}\n" +
                $"Абсолютная фактическая погрешность: {Math.Abs(integral.Trapezoidal - precise).Format()}\n" +
                $"Теоретическая погрешность: {theoreticalError(TrapezoidalConst, TrapezoidalDegree + 1).Format()}\n\n" +
                $"Формула Симпсона\n" +
                $"Вычисленное приближенное значение: {integral.Simpsons.Format()}\n" + 
                $"Абсолютная фактическая погрешность: {Math.Abs(integral.Simpsons - precise).Format()}\n" +
                $"Теоретическая погрешность: {theoreticalError(SimpsonsConst, SimpsonsDegree + 1).Format()}\n");

            Console.WriteLine($"\nЧтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}
