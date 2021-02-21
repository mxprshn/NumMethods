using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public class Matrix
    {
        private double[,] elements;

        public int Height => elements.GetLength(0);
        public int Width => elements.GetLength(1);
        
        public double this[int row, int column] => elements[row, column];

        public Matrix(double[,] elements)
        {
            this.elements = new double[elements.GetLength(0), elements.GetLength(1)];

            for (var i = 0; i < elements.GetLength(0); ++i)
            {
                for (var j = 0; j < elements.GetLength(1); ++j)
                {
                    this.elements[i, j] = elements[i, j];
                }
            }
        }

        public Matrix MultiplyByScalar(double scalar)
        {
            var result = new double[Height, Width];

            for (var i = 0; i < Height; ++i)
            {
                for (var j = 0; j < Width; ++j)
                {
                    result[i, j] = scalar * this[i, j];
                }
            }

            return new Matrix(result);
        }

        public Matrix Add(Matrix other)
        {
            if (other.Width != Width || other.Height != Height)
            {
                throw new ArgumentException("Incompatible matrix dimensions.");
            }

            var result = new double[Height, Width];

            for (var i = 0; i < Height; ++i)
            {
                for (var j = 0; j < Width; ++j)
                {
                    result[i, j] = other[i, j] + this[i, j];
                }
            }

            return new Matrix(result);
        }

        public Matrix RightMultiply(Matrix other)
        {
            if (other.Width != Height || other.Height != Width)
            {
                throw new ArgumentException("Incompatible matrix dimensions.");
            }

            var result = new double[Height, other.Width];

            for (var i = 0; i < Height; ++i)
            {
                for (var j = 0; j < other.Width; ++j)
                {
                    var sum = 0.0;

                    for (var k = 0; k < Width; ++k)
                    {
                        sum += this[i, k] * other[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return new Matrix(result);
        }

        public static Matrix operator *(double scalar, Matrix matrix)
        {
            return matrix.MultiplyByScalar(scalar);
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            return matrix.MultiplyByScalar(scalar);
        }

        public static Matrix operator *(Matrix one, Matrix another)
        {
            return one.RightMultiply(another);
        }

        public static Matrix operator +(Matrix one, Matrix another)
        {
            return one.Add(another);
        }
    }
}
