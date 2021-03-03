using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    /// <summary>
    /// 2D vector with real elements
    /// </summary>
    public class Vector : Matrix
    {
        public Vector(double[] elements) : base(ToTwoDimensionalArray(elements)) { }

        public int Length => Height;

        public double this[int index] => this[index, 0];

        private static double[,] ToTwoDimensionalArray(double[] oneDimensional)
        {
            var result = new double[oneDimensional.Length, 1];

            for (var i = 0; i < oneDimensional.Length; ++i)
            {
                result[i, 0] = oneDimensional[i];
            }

            return result;
        }
    }
}
