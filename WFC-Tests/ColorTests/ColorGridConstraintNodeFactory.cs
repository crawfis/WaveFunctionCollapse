
using CrawfisSoftware.Collections.Graph;

using System.Numerics;

namespace CrawfisSoftware.WaveFunctionCollapse.Tests
{
    public class ColorGridConstraintNodeFactory<N, M> : IConstraintNodeFactory<Colors, Colors>
    {
        private Grid<N, M> _grid;
        private Colors _initialValue;
        private ISolver<Colors, Colors> _solver;
        private int _initialEntropy;
        private int _nodeUpdateCount = 0;
        private int _redCount = 1;

        // Constructor
        public ColorGridConstraintNodeFactory(Grid<N, M> grid, ISolver<Colors, Colors> solver, Colors initialValue, int redCount = 1)
        {
            _grid = grid;
            _solver = solver;
            _initialValue = initialValue;
            _redCount = redCount;
            _initialEntropy = CalculateEntropy(_initialValue);
        }

        // Create method needed for IConstraintNodeFactory
        public IConstraintNode<Colors, Colors> Create(int nodeIndex)
        {
            var node = new ConstraintEnumNodeWithOracles<Colors, N, M>(nodeIndex, _solver, _initialValue)
            {
                Collapse = CollapseAndUpdateRedCount,
                //node.Collapse = ConstraintNode<Colors, int, int>.CollapseToFirstOption<Colors>;
                Reducer = GatherStateAndReduce,
                Entropy = _initialEntropy
            };
            return node;
        }

        // The actual Collapse method
        private bool CollapseAndUpdateRedCount(Colors possibilities, Random random, out Colors collapsedValue)
        {
            // Example of a custom collapse function that updates the red count
            if (ConstraintOracles.CollapseToRandomOption<Colors>(possibilities, random, out collapsedValue))
            {
                if (collapsedValue == Colors.Red)
                {
                    _redCount--;
                }
                return true;
            }
            return false;
        }

        // The actual Reduce method
        private Colors GatherStateAndReduce(int nodeId, Colors colors)
        {
            if (_redCount <= 0)
            {
                colors &= ~Colors.Red;
            }
            foreach (int neighborIndex in _grid.Neighbors(nodeId))
            {
                Colors neighborColor = _solver.GetNodeValues(neighborIndex);
                // Note: Requires .NET Standard 3.0 or greater - Will not work with Unity 6000.
                // Could Couple the ColorNode with this logic to require it to update Entropy.
                // Then we would just check if Entropy is greater than 1.
                int bitsSet = BitOperations.PopCount((uint)neighborColor);
                if (bitsSet > 1) continue;
                colors &= ~neighborColor;
            }
            _nodeUpdateCount++;
            return colors;
        }

        // Helper function to calculate the Entropy.
        private int CalculateEntropy(Colors colors)
        {
            int bitsSet = BitOperations.PopCount((uint)colors);
            return bitsSet;
        }
    }
}