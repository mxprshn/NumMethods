using System;
using NumAn;

namespace CauchyProblem
{
    class Program
    {
        private static double Derivative(double x, double y)
        {
            return - 3 * y + Math.Pow(y, 2);
        }

        private static double ExpectedSolution(double x)
        {
            return 3.0 / (2 * Math.Exp(3.0 * x) + 1);
        }

        private const double CauchyX = 0.0;
        private const double CauchyY = 1.0;


        private static double SecondDerivative(double x, double y)
        {
            var dy = Derivative(x, y);
            return 2 * y * dy - 3 * dy;
        }

        private static double ThirdDerivative(double x, double y)
        {
            var dy = Derivative(x, y);
            var dy2 = SecondDerivative(x, y);
            return 2 * y * dy2 - 3 * dy2 + 2 * dy * dy;
        }

        private static double FourthDerivative(double x, double y)
        {
            var dy = Derivative(x, y);
            var dy2 = SecondDerivative(x, y);
            var dy3 = ThirdDerivative(x, y);
            return 2 * y * dy3 - 3 * dy3 + 6 * dy * dy2;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #6\n\n" +
                "Численное решение Задачи Коши для обыкновенного дифференциального уравнения первого порядка\n\n" +
                "Вариант #10\n");

            Console.WriteLine("Задача Коши: y'(x) = -3 * y(x) + y^2(x); y(0) = 1\n\n");

            while (CauchyLoop()) { };
        }

        private static bool CauchyLoop()
        {
            Console.Write("Введите длину шага между точками (h): ");

            var segmentLength = 0d;

            while (!double.TryParse(Console.ReadLine(), out segmentLength) || !segmentLength.IsNumber())
            {
                Console.Write("Некорректное значение: введите вещественное число: ");
            }

            Console.Write("Введите N: ");

            var enteredN = 0;

            while (!int.TryParse(Console.ReadLine(), out enteredN) || enteredN < 2)
            {
                Console.Write("Некорректное значение: введите целое число большее или равное 2: ");
            }

            FormatExtensions.PrintDivider();

            var derivatives = new double[5];
            derivatives[0] = CauchyY;
            derivatives[1] = Derivative(CauchyX, CauchyY);
            derivatives[2] = SecondDerivative(CauchyX, CauchyY);
            derivatives[3] = ThirdDerivative(CauchyX, CauchyY);
            derivatives[4] = FourthDerivative(CauchyX, CauchyY);

            var solver = new CauchySolver(Derivative, CauchyX, CauchyY, segmentLength, enteredN + 2, derivatives)

            Console.WriteLine($"Чтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }


    }
}
