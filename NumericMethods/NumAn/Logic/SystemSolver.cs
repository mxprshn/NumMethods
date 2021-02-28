using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NumAn
{
    namespace SystemSolving
    {
        public class SystemSolver
        {
            public static Vector Gauss(Matrix systemMatrix, Vector constantVector)
            {
                if (systemMatrix.Height != systemMatrix.Width)
                {
                    throw new ArgumentException("System matrix must be square");
                }

                if (systemMatrix.Height != constantVector.Height)
                {
                    throw new ArgumentException("Constant vector must have length equal to system matrix size");
                }

                var augmented = CreateAugmentedMatrix(systemMatrix, constantVector);

                Console.WriteLine(augmented.Format());

                for (var i = 0; i < augmented.GetLength(0); ++i)
                {
                    MoveMaxValueSubmatrixRowToTop(augmented, i);

                    Console.WriteLine(augmented.Format());

                    var divisor = augmented[i, i];

                    for (var j = i; j < augmented.GetLength(1); ++j)
                    {
                        augmented[i, j] /= divisor;
                    }

                    for (var j = i + 1; j < augmented.GetLength(0); ++j)
                    {
                        var elementToReduce = augmented[j, i];

                        for (var k = i; k < augmented.GetLength(1); ++k)
                        {
                            augmented[j, k] -= augmented[i, k] * elementToReduce;
                        }
                    }

                    Console.WriteLine(augmented.Format());
                }

                var solution = new double[systemMatrix.Height];

                solution[systemMatrix.Height - 1] = augmented[systemMatrix.Height - 1, systemMatrix.Width];

                for (var i = systemMatrix.Height - 2; i >= 0 ; --i)
                {
                    var sum = augmented[i, systemMatrix.Width];

                    for (var j = i + 1; j < systemMatrix.Width; ++j)
                    {
                        sum -= augmented[i, j] * solution[j];
                    }

                    solution[i] = sum;
                }

                return new Vector(solution);
            }

            public static Matrix Jordan(Matrix systemMatrix, Matrix constantMatrix)
            {
                if (systemMatrix.Height != systemMatrix.Width)
                {
                    throw new ArgumentException("System matrix must be square");
                }

                if (systemMatrix.Height != constantMatrix.Height)
                {
                    throw new ArgumentException("Constant matrix must have height equal to system matrix size");
                }

                var augmented = CreateAugmentedMatrix(systemMatrix, constantMatrix);

                Console.WriteLine(augmented.Format());

                for (var i = 0; i < augmented.GetLength(0); ++i)
                {
                    MoveMaxValueSubmatrixRowToTop(augmented, i);

                    Console.WriteLine(augmented.Format());

                    var divisor = augmented[i, i];

                    for (var j = i; j < augmented.GetLength(1); ++j)
                    {
                        augmented[i, j] /= divisor;
                    }

                    for (var j = 0; j < augmented.GetLength(0); ++j)
                    {
                        if (j == i)
                        {
                            continue;
                        }

                        var elementToReduce = augmented[j, i];

                        for (var k = i; k < augmented.GetLength(1); ++k)
                        {
                            augmented[j, k] -= augmented[i, k] * elementToReduce;
                        }
                    }

                    Console.WriteLine(augmented.Format());
                }

                var solution = new double[constantMatrix.Height, constantMatrix.Width];

                for (var i = 0; i < constantMatrix.Height; ++i)
                {
                    for (var j = systemMatrix.Width; j < augmented.GetLength(1); ++j)
                    {
                        solution[i, j - systemMatrix.Width] = augmented[i, j];
                    }
                }

                return new Matrix(solution);
            }

            private static double[,] CreateAugmentedMatrix(Matrix systemMatrix, Matrix constantMatrix)
            {
                var result = new double[systemMatrix.Height, systemMatrix.Width + constantMatrix.Width];

                for (var i = 0; i < systemMatrix.Height; ++i)
                {
                    for (var j = 0; j < systemMatrix.Width; ++j)
                    {
                        result[i, j] = systemMatrix[i, j];
                    }

                    for (var j = systemMatrix.Width; j < result.GetLength(1); ++j)
                    {
                        result[i, j] = constantMatrix[i, j - systemMatrix.Width];
                    }
                }

                return result;
            }

            private static void MoveMaxValueSubmatrixRowToTop(double[,] matrix, int topLeftCornerIndex)
            {
                var maxValue = Math.Abs(matrix[topLeftCornerIndex, topLeftCornerIndex]);
                var maxValueRowNumber = topLeftCornerIndex;

                for (var j = topLeftCornerIndex + 1; j < matrix.GetLength(0); ++j)
                {
                    var abs = Math.Abs(matrix[j, topLeftCornerIndex]);

                    if (abs > maxValue)
                    {
                        maxValue = abs;
                        maxValueRowNumber = j;
                    }
                }

                if (maxValueRowNumber != topLeftCornerIndex)
                {
                    for (var j = 0; j < matrix.GetLength(1); ++j)
                    {
                        DoubleUtils.Swap(ref matrix[topLeftCornerIndex, j], ref matrix[maxValueRowNumber, j]);
                    }
                }
            }
        }
    }

}
