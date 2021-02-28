using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    namespace Interpolation
    {
        public interface IInterpolationPolynomial
        {
            public Func<double, double> ToFunction();
        }
    }

}
