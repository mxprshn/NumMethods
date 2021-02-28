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

                var buffer = new double[systemMatrix.Height, systemMatrix.Width + 1];

                for (var i = 0; i < systemMatrix.Height; ++i)
                {
                    for (var j = 0; j < systemMatrix.Width; ++j)
                    {
                        buffer[i, j] = systemMatrix[i, j];
                    }
                }

                for (var i = 0; i < buffer.GetLength(0); ++i)
                {
                    buffer[i, systemMatrix.Width] = constantVector[i];
                }

                Console.WriteLine(buffer.Format());

                for (var i = 0; i < buffer.GetLength(0); ++i)
                {
                    var maxValue = Math.Abs(buffer[i, i]);
                    var maxValueRowNumber = i;

                    for (var j = i + 1; j < buffer.GetLength(0); ++j)
                    {
                        var abs = Math.Abs(buffer[j, i]);

                        if (abs > maxValue)
                        {
                            maxValue = abs;
                            maxValueRowNumber = j;
                        }
                    }

                    if (maxValueRowNumber != i)
                    {
                        for (var j = 0; i < buffer.GetLength(1); ++i)
                        {
                            Utils.Swap(ref buffer[i, j], ref buffer[maxValueRowNumber, j]);
                        }
                    }

                    Console.WriteLine(buffer.Format());

                    var divisor = buffer[i, i];

                    for (var j = i; j < buffer.GetLength(1); ++j)
                    {
                        buffer[i, j] /= divisor;
                    }

                    for (var j = i + 1; j < buffer.GetLength(0); ++j)
                    {
                        var elementToReduce = buffer[j, i];

                        for (var k = i; k < buffer.GetLength(1); ++k)
                        {
                            buffer[j, k] -= buffer[i, k] * elementToReduce;
                        }
                    }

                    Console.WriteLine(buffer.Format());
                }

                var solution = new double[systemMatrix.Height];

                solution[systemMatrix.Height - 1] = buffer[systemMatrix.Height - 1, systemMatrix.Width];

                for (var i = systemMatrix.Height - 2; i >= 0 ; --i)
                {
                    var sum = buffer[i, systemMatrix.Width];

                    for (var j = i + 1; j < systemMatrix.Width; ++j)
                    {
                        sum -= buffer[i, j] * solution[j];
                    }

                    solution[i] = sum;
                }

                return new Vector(solution);
            }
        }
    }

}
