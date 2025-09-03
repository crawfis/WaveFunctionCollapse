using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.Collections;
using System.Numerics; // For BitOperations

namespace CrawfisSoftware.WaveFunctionCollapse.Tests
{
    public enum SolverType
    {
        EventBased,
        StateBased
    }

    internal static class WFC_ColorTest
    {
        static Colors colorChoices;
        static Colors sixColors = Colors.Red | Colors.Green | Colors.Blue | Colors.Yellow | Colors.Purple | Colors.Orange;
        static Colors threeColors = Colors.Red | Colors.Green | Colors.Blue;
        static int _redCount = 3;
        private const int Width = 16;
        private const int Height = 16;
        private static SolverType solverType = SolverType.StateBased;
        static Grid<int, int> _grid;
        static ISolver<Colors, Colors> _solver;
        private static int randomSeed = 8976895;

        public static void ColorTest()
        {
            colorChoices = sixColors;
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
                    var value = _solver.GetNodeValues(row * Width + column);
                    WriteColorValue(value, 10);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void WriteColorValue(Colors value, int width)
        {
            // If multiple possibilities remain (more than one flag set), print in gray.
            if (BitOperations.PopCount((uint)value) != 1)
            {
                var previous = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(value.ToString().PadLeft(width));
                Console.ForegroundColor = previous;
                return;
            }
            var color = GetConsoleColor(value);
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            // Print just the single color name (enum ToString ok here)
            Console.Write(value.ToString().PadLeft(width));
            Console.ForegroundColor = old;
        }

        private static ConsoleColor GetConsoleColor(Colors color) => color switch
        {
            Colors.Red => ConsoleColor.Red,
            Colors.Green => ConsoleColor.Green,
            Colors.Blue => ConsoleColor.Blue,
            Colors.Yellow => ConsoleColor.Yellow,
            Colors.Purple => ConsoleColor.Magenta,
            Colors.Orange => ConsoleColor.DarkYellow, // Approximation
            _ => ConsoleColor.White
        };

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
            var _nodeFactory = new ColorGridConstraintNodeFactory<int, int>(grid, solver, colorChoices, _redCount);
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