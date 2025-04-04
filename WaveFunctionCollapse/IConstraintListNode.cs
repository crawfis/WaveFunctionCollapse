using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintListNode<T> : IConstraintNode<T, IList<T>>
    {
        // Works for Arrays and Lists.
    }
}