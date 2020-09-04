using System;
using System.Collections.Generic;
using System.Text;

namespace NonlinearEquations
{
    public class RootFinderException : Exception
    {
        public RootFinderException(string operation, string message) : base($"{operation}: {message}") { }
    }
}
