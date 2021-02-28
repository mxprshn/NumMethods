using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public class MatrixUtils
    {
        public static (Matrix L, Matrix U) LuDecomposition(Matrix input)
        {
            if (input.Height != input.Width)
            {
                throw new ArgumentException("Matrix must be square");
            }

            var lBuffer = new double[input.Height, input.Width];
            var uBuffer = new double[input.Height, input.Width];

            for (var i = 0; i < input.Height; ++i)
            {
                for (var j = 0; j < input.Height; ++j)
                {
                    var sum = input[j, i];
                        
                    for (var k = 0; k < i; ++k)
                    {
                        sum -= lBuffer[j, k] * uBuffer[k, i];
                    }

                    if (i == j && sum == 0.0)
                    {
                        throw new LuZeroDiagonalElementException();
                    }

                    lBuffer[j, i] = sum;
                }

                for (var j = 0; j < input.Height; ++j)
                {
                    var sum = input[i, j];

                    for (var k = 0; k < i; ++k)
                    {
                        sum -= lBuffer[i, k] * uBuffer[k, j];
                    }

                    uBuffer[i, j] = sum / lBuffer[i, i];
                }
            }

            return (new Matrix(lBuffer), new Matrix(uBuffer));
        }

        [Serializable]
        public class LuZeroDiagonalElementException : Exception
        {
            public LuZeroDiagonalElementException() { }
            public LuZeroDiagonalElementException(string message) : base(message) { }
            public LuZeroDiagonalElementException(string message, Exception inner) : base(message, inner) { }
            protected LuZeroDiagonalElementException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
