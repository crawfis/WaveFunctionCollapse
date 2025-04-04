using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class EntropyComparer<T, TChoices> : IComparer<IConstraintNode<T, TChoices>>
    {
        public int Compare(IConstraintNode<T, TChoices> x, IConstraintNode<T, TChoices> y)
        {
            return x.Entropy.CompareTo(y.Entropy);
        }
    }
}