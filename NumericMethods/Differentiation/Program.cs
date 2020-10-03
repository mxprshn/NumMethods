using System;
using NumAn;
using NumAn.Differentiation;


namespace Differentiation
{
    class Program
    {
        static double Function(double x)
        {
            // return 2 * Math.Pow(x, 2) + 10 * Math.Pow(x, 4);
            return Math.Exp(1.5 * x);
        }

        static double FirstDerivative(double x)
        {
            // return 2 * Math.Pow(x, 2) + 10 * Math.Pow(x, 4);
            return 1.5 * Math.Exp(1.5 * x);
        }

        static double SecondDerivative(double x)
        {
            // return 2 * Math.Pow(x, 2) + 10 * Math.Pow(x, 4);
            return 2.25 * Math.Exp(1.5 * x);
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа #3.2\n\n" +
                "Нахождение производных таблично-заданной функции по формулам численного дифференцирования\n\n" +
                "Вариант #10\n");

            while (Differentiation()) { };

        }

        private static bool Differentiation()
        {
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

            Console.Write("Введите длину шага: ");

            var segmentLength = 0.0;

            while (!double.TryParse(Console.ReadLine(), out segmentLength) || segmentLength <= 0)
            {
                Console.Write("Некорректное значение: введите положительное вещественное число: ");
            }

            var functionTable = new (double x, double y)[numberOfValues];

            for (var i = 0; i < numberOfValues; ++i)
            {
                var x = start + segmentLength * i;
                functionTable[i] = (x, Function(x));
            }

            Console.WriteLine($"\nТаблица значений функции (число значений: {numberOfValues})");
            Console.WriteLine(functionTable.Format("x", "f(x)"));

            var differentiator = new Differentiator(functionTable);
            var firstDerivative = differentiator.FirstDerivative();
            var secondDerivative = differentiator.SecondDerivative();

            var resultTable = new string[functionTable.Length + 1, 6];

            resultTable[0, 0] = "x_i";
            resultTable[0, 1] = "f(x_i)";
            resultTable[0, 2] = "f'(x_i)_ЧД";
            resultTable[0, 3] = "|f'(x_i)_Т - f'(x_i)_ЧД|";
            resultTable[0, 4] = "f''(x_i)_ЧД";
            resultTable[0, 5] = "|f''(x_i)_Т - f''(x_i)_ЧД|";

            for (var i = 1; i < resultTable.GetLength(0); ++i)
            {
                resultTable[i, 0] = functionTable[i - 1].x.Format();
                resultTable[i, 1] = functionTable[i - 1].y.Format();
                resultTable[i, 2] = firstDerivative[i - 1].y.Format();
                resultTable[i, 3] = Math.Abs(firstDerivative[i - 1].y - FirstDerivative(functionTable[i - 1].x)).Format();
                resultTable[i, 4] = i == 1 || i == resultTable.GetLength(0) - 1 ? "--" : secondDerivative[i - 2].y.Format();
                resultTable[i, 5] = i == 1 || i == resultTable.GetLength(0) - 1 ? "--" :
                    Math.Abs(secondDerivative[i - 2].y - SecondDerivative(functionTable[i - 1].x)).Format();

            }

            Console.WriteLine(resultTable.Format(50));

            Console.WriteLine($"\nЧтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}
