using System;
using NumAn;

namespace Integration
{
    class Program
    {
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



            Console.WriteLine($"\nЧтобы выйти, нажмите \'Esc\'\n");

            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}
