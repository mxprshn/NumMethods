using System;
using NumAn;
using System.Linq;

namespace CauchyProblem
{
    class Program
    {
        private static double Derivative(double x, double y)
        {
            return -3 * y + Math.Pow(y, 2);
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

            Console.WriteLine($"Длина шага (h): {segmentLength.Format()}");
            Console.WriteLine($"N: {enteredN}");

            FormatExtensions.PrintDivider();

            Console.WriteLine("Точное решение ОДУ: \n");

            var expectedValues = new double[2][];
            var nodes = new double[enteredN + 3];
            expectedValues[1] = new double[enteredN + 3];
            expectedValues[0] = nodes;

            for (var i = -2; i <= enteredN; ++i)
            {
                nodes[i + 2] = CauchyX + i * segmentLength;
                expectedValues[1][i + 2] = ExpectedSolution(nodes[i + 2]);
            }

            expectedValues.Format(Enumerable.Range(-2, enteredN + 3).Select(n => n.ToString()).ToArray(), new[] { "x", "y" });

            FormatExtensions.PrintDivider();

            var derivatives = new double[5];
            derivatives[0] = CauchyY;
            derivatives[1] = Derivative(CauchyX, CauchyY);
            derivatives[2] = SecondDerivative(CauchyX, CauchyY);
            derivatives[3] = ThirdDerivative(CauchyX, CauchyY);
            derivatives[4] = FourthDerivative(CauchyX, CauchyY);

            var solver = new CauchySolver(Derivative, CauchyX, CauchyY, segmentLength, enteredN + 2, derivatives);

            Console.WriteLine($"Найденные значения производных f(x, y) в точке {CauchyX.Format()}\n");

            for (var i = 0; i < derivatives.Length; ++i)
            {
                Console.WriteLine($"f^({i}) = {derivatives[i].Format()}");
            }

            Console.WriteLine();

            var taylor = new double[3][];
            taylor[0] = nodes;
            taylor[1] = solver.Taylor();
            taylor[2] = new double[enteredN + 3];

            for (var i = 0; i < expectedValues[1].Length; ++i)
            {
                taylor[2][i] = Math.Abs(taylor[1][i] - expectedValues[1][i]);
            }            

            Console.WriteLine("Метод разложения в ряд Тейлора: \n");

            taylor.Format(Enumerable.Range(-2, enteredN + 3).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            var adams = new double[3][];
            adams[0] = nodes.TakeLast(enteredN - 2).ToArray();
            adams[1] = solver.Adams(expectedValues[1].Take(5).ToArray()).TakeLast(enteredN - 2).ToArray();
            adams[2] = new double[enteredN - 2];

            var expectedValuesTrimmed = expectedValues[1].TakeLast(enteredN - 2).ToArray();

            for (var i = 0; i < expectedValuesTrimmed.Length; ++i)
            {
                adams[2][i] = Math.Abs(expectedValuesTrimmed[i] - adams[1][i]);
            }

            Console.WriteLine("Экстраполяционный метод Адамса 4-го порядка: \n");

            adams.Format(Enumerable.Range(3, enteredN - 2).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            Console.WriteLine("Метод Рунге-Кутта 4-го порядка: \n");

            var rungeKutta = new double[3][];
            rungeKutta[0] = nodes.TakeLast(enteredN).ToArray();
            rungeKutta[1] = solver.RungeKutta().TakeLast(enteredN).ToArray();
            rungeKutta[2] = new double[enteredN];

            expectedValuesTrimmed = expectedValues[1].TakeLast(enteredN).ToArray();

            for (var i = 0; i < expectedValuesTrimmed.Length; ++i)
            {
                rungeKutta[2][i] = Math.Abs(expectedValuesTrimmed[i] - rungeKutta[1][i]);
            }

            rungeKutta.Format(Enumerable.Range(1, enteredN).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            Console.WriteLine("Метод Эйлера: \n");

            var euler = new double[3][];
            euler[0] = nodes.TakeLast(enteredN).ToArray();
            euler[1] = solver.Euler().TakeLast(enteredN).ToArray();
            euler[2] = new double[enteredN];

            for (var i = 0; i < expectedValuesTrimmed.Length; ++i)
            {
                euler[2][i] = Math.Abs(expectedValuesTrimmed[i] - euler[1][i]);
            }

            euler.Format(Enumerable.Range(1, enteredN).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            Console.WriteLine("Метод Эйлера I: \n");

            var euler1 = new double[3][];
            euler1[0] = nodes.TakeLast(enteredN).ToArray();
            euler1[1] = solver.EulerI().TakeLast(enteredN).ToArray();
            euler1[2] = new double[enteredN];

            for (var i = 0; i < expectedValuesTrimmed.Length; ++i)
            {
                euler1[2][i] = Math.Abs(expectedValuesTrimmed[i] - euler1[1][i]);
            }

            euler1.Format(Enumerable.Range(1, enteredN).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            Console.WriteLine("Метод Эйлера II: \n");

            var euler2 = new double[3][];
            euler2[0] = nodes.TakeLast(enteredN).ToArray();
            euler2[1] = solver.EulerII().TakeLast(enteredN).ToArray();
            euler2[2] = new double[enteredN];

            for (var i = 0; i < expectedValuesTrimmed.Length; ++i)
            {
                euler2[2][i] = Math.Abs(expectedValuesTrimmed[i] - euler2[1][i]);
            }

            euler2.Format(Enumerable.Range(1, enteredN).Select(n => n.ToString()).ToArray(), new[] { "x", "y", "погрешность" });

            FormatExtensions.PrintDivider();

            Console.WriteLine("Абслоютная фактическая погрешность для y_N: \n");

            Console.WriteLine($"Метод разложения в ряд Тейлора: {Math.Abs(expectedValues[1].Last() - taylor[1].Last()).Format()}");
            Console.WriteLine($"Экстраполяционный метод Адамса 4-го порядка: {Math.Abs(expectedValues[1].Last() - adams[1].Last()).Format()}");
            Console.WriteLine($"Метод Рунге-Кутта 4-го порядка: {Math.Abs(expectedValues[1].Last() - rungeKutta[1].Last()).Format()}");
            Console.WriteLine($"Метод Эйлера: {Math.Abs(expectedValues[1].Last() - euler[1].Last()).Format()}");
            Console.WriteLine($"Метод Эйлера I: {Math.Abs(expectedValues[1].Last() - euler1[1].Last()).Format()}");
            Console.WriteLine($"Метод Эйлера II: {Math.Abs(expectedValues[1].Last() - euler2[1].Last()).Format()}\n");

            Console.WriteLine($"Чтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }


    }
}
