using System.Collections.Generic;

namespace NonlinearEquations
{
    /// <summary>
    /// Результат уточнения корней.
    /// </summary>
    public class IntervalResult
    {
        /// <summary>
        /// Число итераций.
        /// </summary>
        public int IterationCount { get; set; }

        /// <summary>
        /// Приближенное решение.
        /// </summary>
        public double Root { get; set; }

        /// <summary>
        /// Абсолютная величина невязки.
        /// </summary>
        public double AbsoluteResidual { get; set; }

        /// <summary>
        /// Начальные приближения.
        /// </summary>
        public List<double> InitialApproximations { get; set; }
    }
}