using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class EdgeStateComparer : IComparer<EdgeState>
    {
        public int Compare(EdgeState x, EdgeState y)
        {
            int pathStyleComparison = x.pathStyle.CompareTo(y.pathStyle);
            if (pathStyleComparison != 0)
            {
                return pathStyleComparison;
            }
            return x.edgeHeight.CompareTo(y.edgeHeight);
        }
    }
}