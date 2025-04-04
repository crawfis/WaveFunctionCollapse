using CrawfisSoftware.Collections;

using System;
using System.Collections;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public abstract class ConstraintBitArrayNode : IConstraintNode<BitArray, BitArray>
    {
        public int Id { get; }
        public bool IsCollapsed { get; protected set; }
        public BitArray Possibilities { get; set; }
        public BitArray CollapsedValue { get; protected set; }
        public int CollapsedIndex { get; protected set; } = -1;
        public int Entropy { get; protected set; }

        public ConstraintBitArrayNode(int id, BitArray possibilities, int initialEntropy)
        {
            Id = id;
            Possibilities = possibilities;
            IsCollapsed = false;
            CollapsedValue = new BitArray(possibilities.Length);
            Entropy = initialEntropy;
        }
        public abstract bool Reduce();

        public abstract bool TryCollapseNode(Random random, out BitArray collapsedValue);
        public virtual void UpdateEntropy()
        {
            Entropy = Possibilities.SetBitsCounts();
        }
    }
}