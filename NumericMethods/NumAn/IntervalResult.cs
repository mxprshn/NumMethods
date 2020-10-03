using System.Collections.Generic;

namespace NumAn
{
    namespace RootFinding
    {
        public class IntervalResult
        {
            public int IterationCount { get; set; }

            public double Root { get; set; }

            public double AbsoluteResidual { get; set; }

            public List<double> InitialApproximations { get; set; }
        }
    }
}