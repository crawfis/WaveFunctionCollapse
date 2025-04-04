using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.Collections;

namespace CrawfisSoftware.WaveFunctionCollapse.Tests
{
    [Flags]
    public enum Colors
    {
        Red = 1,
        Green = 2,
        Blue = 4,
        Yellow = 8,
        Purple = 16,
        Orange = 32
    }
    public enum SolverType
    {
        EventBased,
        StateBased
    }

    internal static class WFC_ColorTest
    {
        static Colors allColors;
        static Colors sixColors = Colors.Red | Colors.Green | Colors.Blue | Colors.Yellow | Colors.Purple | Colors.Orange;
        static Colors threeColors = Colors.Red | Colors.Green | Colors.Blue;
        private const int Width = 16;
        private const int Height = 16;
        private static SolverType solverType = SolverType.StateBased;
        static Grid<int, int> _grid;
        static ISolver<Colors, Colors> _solver;
        private static int randomSeed = 1234;

        public static void ColorTest()
        {
            allColors = sixColors;
            Console.WriteLine($"Hello WFC World! You have {Width * Height} nodes in your graph.");
            _grid = new Grid<int, int>(Width, Height, null, null);
            switch (solverType)
            {
                case SolverType.EventBased:
                    _solver = EventBasedSolver(_grid);
                    break;
                case SolverType.StateBased:
                    _solver = StateBasedSolver(_grid);
                    break;
            }
            _solver.TrySolve(randomSeed, Width * Height);

            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfPropagationCalls} nodes were visited.");
            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfReduceCalls} nodes were reduced.");

            for (int row = Height - 1; row >= 0; row--)
            {
                for (int column = 0; column < Width; column++)
                {
                    Console.Write(_solver.GetNodeValues(row * Width + column) + " ");
                }
                Console.WriteLine();
            }
        }

        private static ISolver<Colors, Colors> StateBasedSolver(Grid<int, int> grid)
        {
            SolverWithOracles<Colors, Colors> solver = CreateSolver(grid);
            solver.ReduceStrategy = new UpdateAllReduceStrategy<Colors, Colors>();
            return solver;
        }

        private static ISolver<Colors, Colors> EventBasedSolver(Grid<int, int> grid)
        {
            SolverWithOracles<Colors, Colors> solver = CreateSolver(grid);
            var strategy = new GraphReduceStrategy<Colors, Colors, int, int>(grid);
            solver.ReduceStrategy = strategy;
            foreach (var node in solver.Nodes)
            {
                foreach (var neighbor in grid.Neighbors(node.Id))
                {
                    strategy.AddGraphConstraint(node.Id, neighbor);
                }
            }

            return solver;
        }

        private static SolverWithOracles<Colors, Colors> CreateSolver(Grid<int, int> grid)
        {
            // Initialize nodes with possible choices.
            SolverWithOracles<Colors, Colors> solver = new SolverWithOracles<Colors, Colors>();
            var nodes = new List<IConstraintNode<Colors, Colors>>();
            var _nodeFactory = new ColorGridConstraintNodeFactory<int, int>(grid, solver, allColors);
            foreach (int nodeIndex in grid.Nodes)
            {
                var node = _nodeFactory.Create(nodeIndex);
                nodes.Add(node);
            }

            //solver.OnNodeCollapsed += Solver_OnNodeCollapsed;
            solver.Initialize(nodes);
            var heapNodes = new Heap<IConstraintNode<Colors, Colors>>(new EntropyComparer<Colors, Colors>());
            foreach (var node in solver.Nodes)
            {
                heapNodes.Add(node);
            }
            solver.NodeSelector = (index, solver) => heapNodes.RemoveRoot().Id;
            return solver;
        }

        //static int redCount = 3;
        //private static void Solver_OnNodeCollapsed(int nodeId, Colors collapsedColor)
        //{
        //    if (collapsedColor == Colors.Red)
        //    {
        //        redCount--;
        //        if (redCount == 0)
        //        {
        //            Console.WriteLine("Red is done!");
        //        }
        //    }
        //}
    }
}