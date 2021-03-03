using System;
using NumAn;
using NumAn.SystemSolving;

namespace Problem61
{
    class Program
    {
        private readonly static Matrix systemMatrix1 = new Matrix
        (
            new double[,]
            {
                { -403.15, 200.95 },
                { 1205.70, -604.10 },
            }
        );

        private readonly static Vector constantVector11 = new Vector
        (
            new double[]
            { 200.0, -600.0 }
        );

        private readonly static Vector constantVector12 = new Vector
        (
            new double[]
            { 199.0, -601.0 }
        );

        private readonly static Matrix systemMatrix2 = new Matrix
        (
            new double[,]
            {
                { 12.44310, 1.493550, -3.841638 },
                { 1.493550, 9.449050, 0.893829 },
                { -3.841638, 0.893829, 7.49750 },
            }
        );

        private readonly static Vector constantVector2 = new Vector
        (
            new double[]
            { 5.047556, 5.918212, 2.274843, }
        );

        static void Main(string[] args)
        {
            ConditionNumberProblem();
            FormatExtensions.PrintDivider();
            GaussEliminationProblem();
            FormatExtensions.PrintDivider();
            JordanEliminationProblem();
            FormatExtensions.PrintDivider();
            LuDecompositionProblem();
        }

        private static void ConditionNumberProblem()
        {
            Console.WriteLine($"Source system:\n\nA = \n{systemMatrix1}\nb = \n{constantVector11}");
            var systemSolution = SystemSolver.Gauss(systemMatrix1, constantVector11);

            Console.WriteLine($"Solution:\n\nx = \n{systemSolution}\n");

            Console.WriteLine($"Modified system\n\nA = \n{systemMatrix1}\nb = \n{constantVector12}");
            var modifiedSystemSolution = SystemSolver.Gauss(systemMatrix1, constantVector12);

            Console.WriteLine($"Solution:\n\n~x = \n{modifiedSystemSolution}\n");

            var conditionNumber = systemMatrix1.ConditionNumber();

            var constantVectorDeviation = (constantVector11 - constantVector12).FrobeniusNorm / constantVector11.FrobeniusNorm;
            var solutionRelativeError = (systemSolution - modifiedSystemSolution).FrobeniusNorm / systemSolution.FrobeniusNorm;

            var errorEstimation = conditionNumber * constantVectorDeviation;

            Console.WriteLine($"System matrix condition number: {conditionNumber.Format()}");
            Console.WriteLine($"Solution relative error: {solutionRelativeError.Format()}");
            Console.WriteLine($"Relative error estimation: {errorEstimation.Format()}");
        }

        private static void GaussEliminationProblem()
        {
            Console.WriteLine($"Solving system with Gauss elimination:\n\nA = \n{systemMatrix2}\nb = \n{constantVector2}");
            var solution = SystemSolver.Gauss(systemMatrix2, constantVector2);
            Console.WriteLine($"Solution:\n\n~x = \n{solution}");
        }

        private static void JordanEliminationProblem()
        {
            Console.WriteLine($"Finding inverse with Jordan elimination for matrix:\n\nA = \n{systemMatrix2}");
            var inverse = systemMatrix2.Inverse();
            Console.WriteLine($"Inverse:\n\nA^-1 = \n{inverse}");
            var expectedIdentity = systemMatrix2 * inverse;
            Console.WriteLine($"Product of A and found inverse:\n\nA * A^-1 = \n{expectedIdentity}");
        }

        private static void LuDecompositionProblem()
        {
            Console.WriteLine($"Finding determinant with LU-decomposition for matrix:\n\nA = \n{systemMatrix2}");
            var determinant = systemMatrix2.Determinant;
            Console.WriteLine($"Determinant:\n\n|A| = {determinant}");
        }
    }
}