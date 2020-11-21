using System;
using System.Collections.Generic;
using System.Text;

namespace NumAn
{
    public static class Functions
    {
        public static Func<double, double> One => x => 1;

        public static Func<double, double> Id => x => x;

        public static Func<double, double> Monomial2 => x => Math.Pow(x, 2);

        public static Func<double, double> Monomial3 => x => Math.Pow(x, 3);

        public static Func<double, double> Monomial4 => x => Math.Pow(x, 4);

        public static Func<double, double> Polynomial3 => x => 5 * Math.Pow(x, 3) + 4 * Math.Pow(x, 2) + 1.0;
    }
}
