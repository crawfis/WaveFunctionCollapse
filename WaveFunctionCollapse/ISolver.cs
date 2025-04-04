using CrawfisSoftware.Collections.Graph;

using System;
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public delegate bool CollapseDelegate<T>(T possibilities, System.Random random, out T collapsedValue);
    public interface ISolver<T, TChoices, N, M>
    {
        IEnumerable<IConstraintNode<T, TChoices>> Nodes { get; }
        public IReduceStrategy<T, TChoices, N, M> ReduceStrategy { get; set; }

        event Action<int, T> OnNodeCollapseEnded;
        event Action<int, TChoices> OnNodeCollapseStarting;

        TChoices GetNodeValues(int index);
        void Initialize(IIndexedGraph<N, M> graph, IConstraintNodeFactory<T, TChoices> nodeFactory);
        void NodeUpdated(int id);
        bool TrySolve(int randomSeed, int numberToCollapse = int.MaxValue);
        IConstraintNode<T, TChoices> GetNode(int index);
    }
}