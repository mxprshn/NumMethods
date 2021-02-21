using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public class Vector : Matrix
    {
        public Vector(double[] elements) : base(ToTwoDimensionalArray(elements)) { }

        private static double[,] ToTwoDimensionalArray(double[] oneDimensional)
        {
            var result = new double[oneDimensional.Length, 1];

            for (var i = 0; i < oneDimensional.Length; ++i)
            {
                result[i, 1] = oneDimensional[i];
            }

            return result;
        }
    }
}
