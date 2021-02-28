using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace SystemSolving
    {
        public class GaussSystemSolver
        {
            public static Vector Solve(Matrix systemMatrix, Vector constantVector)
            {
                if (systemMatrix.Height != systemMatrix.Width)
                {
                    throw new ArgumentException("System matrix must be square");
                }

                if (systemMatrix.Height != constantVector.Height)
                {
                    throw new ArgumentException("Constant vector must have length equal to system matrix size");
                }


            }
        }
    }

}
