using CrawfisSoftware.WaveFunctionCollapse;

using System;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    /// <summary>
    /// A  Constraint Node that uses a CollapseDelegate to collapse and a function to Reduce. These are settable by Properties.
    /// </summary>
    public class ConstraintEnumNodeWithOracles<T, N, M> : IConstraintEnumNode<T> where T : struct, System.Enum
    {
        private ISolver<T, T> _solver;

        public int Id { get; }
        public T Possibilities { get; private set; }
        public T CollapsedValue { get { return Possibilities; } }
        public bool IsCollapsed { get; private set; } = false;
        public int Entropy
        {
            // BitOperations is defined .NET Core 3.0 and later
            //get { return BitOperations.PopCount((uint)Possibilities); }
            get; set;
        } = 1;

        public CollapseDelegate<T> Collapse { get; set; } = ConstraintOracles.CollapseToFirstOption<T>;
        public Func<int, T, T> Reducer { get; set; } = ConstraintOracles.NoOpReducer<T>;

        public ConstraintEnumNodeWithOracles(int nodeIndex, ISolver<T, T> solver, T initialPossibilities, int initialEntropy = 1)
        {
            this.Id = nodeIndex;
            this._solver = solver;
            Possibilities = initialPossibilities;
        }

        public bool TryCollapseNode(System.Random random, out T collapsedValue)
        {
            if (!IsCollapsed && Collapse(Possibilities, random, out T collapsedPossibilities))
            {
                Possibilities = collapsedPossibilities;
                IsCollapsed = true;
                collapsedValue = collapsedPossibilities;
                return true;
            }
            collapsedValue = Possibilities;
            return false;
        }

        public bool Reduce()
        {
            if (!IsCollapsed)
            {
                // Do something with the possibilities
                var newPossibilities = Reducer(Id, Possibilities);
                if (!newPossibilities.Equals(Possibilities))
                {
                    Possibilities = newPossibilities;
                    _solver.NodeUpdated(Id);
                    return true;
                }
            }
            return false;
        }

        public virtual void UpdateEntropy()
        {
            int flagsSet = 0;
            foreach (T flag in Enum.GetValues(typeof(T)))
            {
                if (Possibilities.HasFlag(flag))
                {
                    flagsSet++;
                }
            }
            Entropy = flagsSet;
        }
    }
}
