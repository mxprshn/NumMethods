using System;
using NumAn;
using NumAn.SystemSolving;

namespace Problem61
{
    class Program
    {
        static void Main(string[] args)
        {
            var systemMatrix = new Matrix
            (new double[,] 
                {
                    { 12.44310, 1.493550, -3.841638 },
                    { 1.493550, 9.449050, 0.893829 },
                    { -3.841638, 0.893829, 7.49750 }
                }
            );

            var constantVector = new Vector
            (new double[]
                { 5.047556, 5.918212, 2.274843 }
            );

            var solution = SystemSolver.Gauss(systemMatrix, constantVector);

            Console.WriteLine(solution);

            Console.WriteLine(systemMatrix * solution);
        }
    }
}
