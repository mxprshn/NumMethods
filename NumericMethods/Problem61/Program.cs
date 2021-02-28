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
                    { -3.841638, 0.893829, 7.49750 },
                    { 12.44310, 1.493550, -3.841638 },
                    { 1.493550, 9.449050, 0.893829 },                    
                }
            );

            var constantVector = new Vector
            (new double[]
                { 2.274843, 5.047556, 5.918212,  }
            );

            var solution = SystemSolver.Jordan(systemMatrix, constantVector);

            Console.WriteLine(solution);

            Console.WriteLine(systemMatrix * solution);

            Console.WriteLine(systemMatrix.Determinant);

            Console.WriteLine(systemMatrix.Inverse());

            Console.WriteLine(systemMatrix * systemMatrix.Inverse());
        }
    }
}