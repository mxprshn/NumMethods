using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    /// <summary>
    /// 2D matrix with real elements
    /// </summary>
    public class Matrix
    {
        private double[,] elements;

        /// <summary>
        /// Number of rows in the matrix
        /// </summary>
        public int Height => elements.GetLength(0);

        /// <summary>
        /// Number of columns in matrix
        /// </summary>
        public int Width => elements.GetLength(1);
        
        public double this[int row, int column] => elements[row, column];

        private double? determinant = null;

        /// <summary>
        /// Calculates if needed and returns the determinant of the matrix. Determinant is calculated with LU-decomposition
        /// </summary>
        public double Determinant
        {
            get
            {
                if (determinant.HasValue)
                {
                    return determinant.Value;
                }

                try
                {
                    var (l, u) = MatrixUtils.LuDecomposition(this);

                    var product = 1.0;

                    for (var i = 0; i < l.Height; ++i)
                    {
                        product *= l[i, i];
                    }

                    determinant = product;
                }
                catch (MatrixUtils.LuZeroDiagonalElementException)
                {
                    determinant = 0.0;
                }

                return determinant.Value;
            }
        }

        private double? frobeniusNorm = null;

        /// <summary>
        /// Calculates if needed and returns the Frobenius norm of the matrix
        /// </summary>
        public double FrobeniusNorm
        {
            get
            {
                if (frobeniusNorm.HasValue)
                {
                    return frobeniusNorm.Value;
                }

                var sum = 0.0;

                for (var i = 0; i < Height; ++i)
                {
                    for (var j = 0; j < Width; ++j)
                    {
                        sum += Math.Pow(Math.Abs(elements[i, j]), 2.0);
                    }
                }

                frobeniusNorm = Math.Sqrt(sum);

                return frobeniusNorm.Value;
            }
        }

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

        private Matrix inverse = null;

        /// <summary>
        /// Calculates if needed and returns inverse for the matrix. Inverse is calculated with Jordan decomposition
        /// </summary>
        /// <exception cref="SingularMatrixException">Matrix is singular and has no invrese</exception>
        /// <returns>Inverse for the matrix</returns>
        public Matrix Inverse()
        {
            if (Height != Width)
            {
                throw new ArgumentException("System matrix must be square");
            }

            if (Determinant == 0.0)
            {
                throw new SingularMatrixException();
            }

            if (inverse != null)
            {
                return inverse;
            }

            var identity = Identity(Height);
            inverse = SystemSolving.SystemSolver.Jordan(this, identity);

            return inverse;
        }

        private double? conditionNumber = null;

        /// <summary>
        /// Calculates if needed and returns condition number of the matrix
        /// </summary>
        /// <returns>Condition number of the matrix</returns>
        public double ConditionNumber()
        {
            if (conditionNumber.HasValue)
            {
                return conditionNumber.Value;
            }

            conditionNumber = FrobeniusNorm * Inverse().FrobeniusNorm;
            return conditionNumber.Value;
        }

        /// <summary>
        /// Multiplies elements of the matrix by scalar real value
        /// </summary>
        /// <param name="scalar">Scalar to multiply matrix to</param>
        /// <returns>Result matrix</returns>
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

        /// <summary>
        /// Adds a matrix to this matrix
        /// </summary>
        /// <param name="other">Matrix to add</param>
        /// <returns>Sum of matrices</returns>
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

        /// <summary>
        /// Right multiply this matrix by another 
        /// </summary>
        /// <param name="other">Matrix to multiply this matrix to</param>
        /// <returns>Matrix product</returns>
        public Matrix RightMultiply(Matrix other)
        {
            if (other.Height != Width)
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

        public override string ToString() => elements.Format();

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

        public static Matrix operator -(Matrix one, Matrix another)
        {
            return one.Add(-1 * another);
        }

        /// <summary>
        /// Creates identity matrix of given size
        /// </summary>
        /// <param name="size">Size of the matrix to create</param>
        /// <returns>Identity matrix</returns>
        public static Matrix Identity(int size)
        {
            var elements = new double[size, size];

            for (var i = 0; i < size; ++i)
            {
                elements[i, i] = 1.0;
            }

            return new Matrix(elements);
        }

        [Serializable]
        public class SingularMatrixException : Exception
        {
            public SingularMatrixException() { }
            public SingularMatrixException(string message) : base(message) { }
            public SingularMatrixException(string message, Exception inner) : base(message, inner) { }
            protected SingularMatrixException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
