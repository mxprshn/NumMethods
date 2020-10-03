
namespace NumAn
{
    namespace RootFinding
    {
        public interface IRootFinder
        {
            public IntervalResult FindRoots((double Start, double End) interval, double epsilon);
        }
    }
}