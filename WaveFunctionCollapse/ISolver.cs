using System;
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public delegate bool CollapseDelegate<T>(T possibilities, System.Random random, out T collapsedValue);
    public interface ISolver<T, TChoices>
    {
        IEnumerable<IConstraintNode<T, TChoices>> Nodes { get; }
        public IReduceStrategy<T, TChoices> ReduceStrategy { get; set; }

        event Action<int, TChoices> OnNodeCollapseStarting;
        event Action<int, T> OnNodeCollapseEnded;

        TChoices GetNodeValues(int index);
        void Initialize(List<IConstraintNode<T, TChoices>> constraintNodes);
        void NodeUpdated(int id);
        bool TrySolve(int randomSeed, int numberToCollapse = int.MaxValue);
        IConstraintNode<T, TChoices> GetNode(int index);
    }
}