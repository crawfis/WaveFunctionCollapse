using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class WangTileConstraintNode<TEdge, TTile> : IConstraintListNode<TTile> where TTile : IWangTile<TEdge>
    {
        public const int Left = 0;
        public const int Top = 1;
        public const int Right = 2;
        public const int Bottom = 3;
        private ISolver<TTile, IList<TTile>> _solver;
        public int Id { get; }
        public bool IsCollapsed { get; private set; }
        public IList<TTile> Possibilities { get; private set; }
        public TTile CollapsedValue { get; private set; }
        public int Entropy { get { return Possibilities.Count; } }
        public IComparer<TEdge> EdgeComparer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public WangTileConstraintNode(int id, ISolver<TTile, IList<TTile>> solver, IList<TTile> initialPossibilities, IComparer<TEdge> edgeComparer)
        {
            Id = id;
            IsCollapsed = false;
            Possibilities = new List<TTile>(initialPossibilities);
            CollapsedValue = default;
            _solver = solver;
            EdgeComparer = edgeComparer;
        }

        public bool TryCollapseNode(System.Random random, out TTile collapsedValue)
        {
            if (Possibilities.Count == 0)
            {
                // Todo: Notify the solver that this node has no possibilities left
                //throw new System.ApplicationException($"Node {Id} has no possibilities left");
                collapsedValue = default;
                return false;
            }
            if (IsCollapsed)
            {
                collapsedValue = CollapsedValue;
                return false; // Already Collapsed - should never happen
            }
            collapsedValue = Possibilities[random.Next(Possibilities.Count)];
            IsCollapsed = true;
            CollapsedValue = collapsedValue;
            Possibilities = new List<TTile>() { collapsedValue };
            return true;
        }
        public bool Reduce()
        {
            if (!ReduceBasedOnEdges())
                return false;
            if (!CheckAdditionalConstraints())
                return false;
            return true;
        }

        protected virtual bool CheckAdditionalConstraints()
        {
            return true;
        }

        protected virtual bool ReduceBasedOnEdges()
        {
            bool changed = false;
            var indicesToRemove = new List<int>();
            for (int i = Possibilities.Count - 1; i >= 0; i--)
            {
                var possibility = Possibilities[i];
                if (ValidateTileChoice(possibility)) continue;
                indicesToRemove.Add(i);
                changed = true;
            }
            if (changed)
            {
                foreach (var index in indicesToRemove)
                {
                    Possibilities.RemoveAt(index);
                }
            }
            return changed;
        }

        protected virtual bool ValidateTileChoice(TTile possibility)
        {
            //return true;
            var leftEdge = possibility[Left];
            var topEdge = possibility[Top];
            var rightEdge = possibility[Right];
            var bottomEdge = possibility[Bottom];
            int row = Id / Width;
            int column = Id % Width;

            WangTileConstraintNode<TEdge, TTile> tile;
            if (column > 0)
            {
                tile = _solver.GetNode(Id - 1) as WangTileConstraintNode<TEdge, TTile>;
                if (!CheckEdge(leftEdge, tile, Right)) return false;
            }

            if (column < Width - 1)
            {
                tile = _solver.GetNode(Id + 1) as WangTileConstraintNode<TEdge, TTile>;
                if (!CheckEdge(rightEdge, tile, Left)) return false;
            }

            if (row > 0)
            {
                tile = _solver.GetNode(Id - Width) as WangTileConstraintNode<TEdge, TTile>;
                if (!CheckEdge(bottomEdge, tile, Top)) return false;
            }

            if (row < Height - 1)
            {
                tile = _solver.GetNode(Id + Width) as WangTileConstraintNode<TEdge, TTile>;
                if (!CheckEdge(topEdge, tile, Bottom)) return false;
            }
            return true;
        }

        protected virtual bool CheckEdge(TEdge edgeToCheck, WangTileConstraintNode<TEdge, TTile> tile, int edgeIndex)
        {
            foreach (var possibilities in tile.Possibilities)
            {
                var edge = possibilities[edgeIndex];
                //if (edge.pathStyle == edgeToCheck.pathStyle && edge.edgeHeight == edgeToCheck.edgeHeight)
                if (EdgeComparer.Compare(edge, edgeToCheck) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateEntropy()
        {
        }
    }
}