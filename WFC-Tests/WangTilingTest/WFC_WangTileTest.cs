using CrawfisSoftware.Collections;
using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.WaveFunctionCollapse.Tests;

using System.Diagnostics;
using System.Text;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    enum ReducerType
    {
        BruteForceUpdateAllNodes,
        GraphPropagation
    }
    public enum EdgeHeight { Low, Mid, High, Impassible };
    public enum PathStyle { None, Road, Dirt, Water, Overpass };
    public struct EdgeState
    {
        public PathStyle pathStyle;
        public EdgeHeight edgeHeight;
    }
    public class TileState : IWangTile<EdgeState>
    {
        public EdgeState[] edges;

        public EdgeState GetEdge(IWangTile<EdgeState>.EdgeLocation edge)
        {
            return edges[(int)edge];
        }

        public EdgeState this[int index]
        {
            get
            {
                return edges[index];
            }
        }
    }
    public partial class WFC_WangTileTest
    {
        const int Left = 0;
        const int Top = 1;
        const int Right = 2;
        const int Bottom = 3;


        private const int Width = 19;
        private const int Height = 13;
        static Grid<int, int> _grid;
        static ISolver<TileState, IList<TileState>> _solver;
        private static List<TileState> _tileSet;
        private static int randomSeed = 876;
        public static void MazeTest()
        {
            Console.WriteLine("Hello WFC World! A non-sensical maze for you");
            Console.WriteLine();
            Console.WriteLine("Solving using graph-based solver");
            _grid = new Grid<int, int>(Width, Height, null, null);

            _tileSet = TileSetCreation.CreateTileSet();

            _solver = CreateSolver(_grid, ReducerType.GraphPropagation);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool solved = _solver.TrySolve(randomSeed, Width * Height);
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;

            int tilesUsed = _solver.Nodes
                    .Where(node => node.IsCollapsed)
                    .Select(node => node.CollapsedValue)
                    .Distinct()
                    .Count();

            Console.Write($"Graph-based Solver took {elapsed.Seconds} seconds to solve.");
            Console.WriteLine($"Grid size is {Width} x {Height} = {Width * Height} tiles.");
            Console.WriteLine($"Only {tilesUsed} tiles were used out of {_tileSet.Count} tiles in the set.");
            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfPropagationCalls} nodes were visited.");
            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfReduceCalls} nodes were reduced.");
            PrintSolution();

            Console.WriteLine();
            Console.WriteLine("Now using brute-force solver");
            _solver = CreateSolver(_grid, ReducerType.BruteForceUpdateAllNodes);

            stopwatch = new Stopwatch();
            stopwatch.Start();
            solved = _solver.TrySolve(randomSeed, Width * Height);
            stopwatch.Stop();
            elapsed = stopwatch.Elapsed;

            tilesUsed = _solver.Nodes
                    .Where(node => node.IsCollapsed)
                    .Select(node => node.CollapsedValue)
                    .Distinct()
                    .Count();
            Console.Write($"Brute-Force Solver took {elapsed.Seconds} seconds to solve.");
            Console.WriteLine($"Grid size is {Width} x {Height} = {Width * Height} tiles.");
            Console.WriteLine($"Only {tilesUsed} tiles were used out of {_tileSet.Count} tiles in the set.");
            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfPropagationCalls} nodes were visited.");
            Console.WriteLine($"{_solver.ReduceStrategy.NumberOfReduceCalls} nodes were reduced.");
            PrintSolution();
        }

        private static SolverWithOracles<TileState, IList<TileState>> CreateSolver(Grid<int, int> grid, ReducerType reducerType)
        {
            // Initialize nodes with possible choices.
            SolverWithOracles<TileState, IList<TileState>> solver = new SolverWithOracles<TileState, IList<TileState>>();
            var nodes = new List<IConstraintNode<TileState, IList<TileState>>>();
            IList<TileState> initialChoices = GetInitialChoices();
            var _nodeFactory = CreateNodeFactory(solver, initialChoices);
            foreach (int nodeIndex in grid.Nodes)
            {
                var node = _nodeFactory.Create(nodeIndex);
                // Perform initial reduction
                RestrictTopEdge(node as WangTileConstraintNode<EdgeState, TileState>);
                RestrictBottomEdge(node as WangTileConstraintNode<EdgeState, TileState>);
                nodes.Add(node);
            }
            //solver.OnNodeCollapsed += Solver_OnNodeCollapsed;
            solver.Initialize(nodes);
            SetNodeSelectorHeap(solver);
            SetReducer(solver, reducerType);
            //solver.RippleWaveCompleted += Solver_RippleWaveCompleted;
            Console.WriteLine("Solver created. Starting initial reduction.");
            bool reducing = true;
            int loopCount = 0;
            while (reducing)
            {
                reducing = false;
                loopCount++;
                foreach (var node in nodes)
                {
                    if (node.Reduce()) reducing = true;
                }
                Console.WriteLine($"Loop {loopCount} completed.");
                reducing = false;
            }
            Console.WriteLine("System is ready to solve.");
            return solver;
        }

        private static void Solver_RippleWaveCompleted(int arg1)
        {
            var distinctEdgeHeights = _solver.Nodes
                .SelectMany(node => node.Possibilities)
                .SelectMany(tile => tile.edges)
                .Select(edge => edge.edgeHeight)
                .Distinct()
                .Count();

            Console.WriteLine($"Number of distinct EdgeHeights: {distinctEdgeHeights}  after ripple {arg1}");
            //var possibilities = _solver.Nodes
            //        .Select(node => node.Possibilities).ToArray();
            //var tiles = possibilities.Select(tile => (tile as TileState).edges[1]).ToArray();
            //var topHeights = tiles.Select(edge => edge.edgeHeight)
            //        .Distinct()
            //        .Count();

            ////.edges[1].edgeHeight)
            ////.Distinct()
            ////.Count();
            //var bottomHeights = _solver.Nodes
            //        .Select(node => node.Possibilities)
            //        .Select(tile => (tile as TileState).edges[3].edgeHeight)
            //        .Distinct()
            //        .Count();
            //Console.WriteLine($"Top heights: {topHeights} Bottom heights: {bottomHeights}");
        }

        private static bool RestrictTopEdge(WangTileConstraintNode<EdgeState, TileState> constrainedWangTile)
        {
            bool reduced = false;
            if ((constrainedWangTile.Id / constrainedWangTile.Width) == 0)
            {
                for (int i = constrainedWangTile.Possibilities.Count - 1; i >= 0; i--)
                {
                    var possibility = constrainedWangTile.Possibilities[i];
                    var edge = possibility[Top];
                    if (edge.edgeHeight == EdgeHeight.Impassible)
                        continue;
                    reduced = true;
                    constrainedWangTile.Possibilities.RemoveAt(i);
                }
            }
            return reduced;
        }

        private static bool RestrictBottomEdge(WangTileConstraintNode<EdgeState, TileState> constrainedWangTile)
        {
            bool reduced = false;
            if ((constrainedWangTile.Id / constrainedWangTile.Width) == constrainedWangTile.Height - 1)
            {
                for (int i = constrainedWangTile.Possibilities.Count - 1; i >= 0; i--)
                {
                    var possibility = constrainedWangTile.Possibilities[i];
                    var edge = possibility[Bottom];
                    if (edge.edgeHeight == EdgeHeight.Low)
                        continue;
                    reduced = true;
                    constrainedWangTile.Possibilities.RemoveAt(i);
                }
            }
            return reduced;
        }

        private static IList<TileState> GetInitialChoices()
        {
            return _tileSet;
        }

        private static void SetReducer(SolverWithOracles<TileState, IList<TileState>> solver, ReducerType reducerType)
        {
            if (reducerType == ReducerType.BruteForceUpdateAllNodes)
            {
                solver.ReduceStrategy = new UpdateAllReduceStrategy<TileState, IList<TileState>>();
                //solver.ReduceStrategy = new WangTileReducerBruteForce();
                return;
            }
            var strategy = new GraphReduceStrategy<TileState, IList<TileState>, int, int>(_grid);
            //strategy.MaxRipples = 5;
            solver.ReduceStrategy = strategy;
            foreach (var node in solver.Nodes)
            {
                foreach (var neighbor in _grid.Neighbors(node.Id))
                {
                    strategy.AddGraphConstraint(node.Id, neighbor);
                }
            }
        }

        private static int FindMinEntropy(int index, ISolver<TileState, IList<TileState>> solver)
        {
            int minIndex = -1;
            int minEntropy = int.MaxValue;
            foreach (var node in solver.Nodes)
            {
                if (node.IsCollapsed)
                    continue;
                int entropy = node.Entropy;
                if (entropy < minEntropy)
                {
                    minIndex = node.Id;
                }
                if (entropy == minEntropy)
                {
                    // Need a random to decide which index to keep.
                }
            }
            return minIndex;
        }
        private static void SetNodeSelector(SolverWithOracles<TileState, IList<TileState>> solver)
        {
            solver.NodeSelector = FindMinEntropy;
        }

        private static Heap<IConstraintNode<TileState, IList<TileState>>> _entropyHeap;
        private static void SetNodeSelectorHeap(SolverWithOracles<TileState, IList<TileState>> solver)
        {
            _entropyHeap = new Heap<IConstraintNode<TileState, IList<TileState>>>(new EntropyComparer<TileState, IList<TileState>>());
            solver.OnPropagateEnding += (s) =>
            {
                RefreshHeap(solver);
            };
            RefreshHeap(solver);
            solver.NodeSelector = (index, solver) => _entropyHeap.RemoveRoot().Id;
        }

        private static void RefreshHeap(SolverWithOracles<TileState, IList<TileState>> solver)
        {
            _entropyHeap.Clear();
            foreach (var node in solver.Nodes)
            {
                if (node.IsCollapsed)
                    continue;
                _entropyHeap.Add(node);
            }
        }

        private static IConstraintNodeFactory<TileState, IList<TileState>> CreateNodeFactory(SolverWithOracles<TileState, IList<TileState>> solver, IList<TileState> initialChoices)
        {
            return new WangTileConstraintNodeFactory<EdgeState, TileState>(solver, initialChoices, new EdgeStateComparer(), Width, Height);
        }

        private static void PrintSolution()
        {
            const string horizontalBar = "-------";
            const string spaceBar = "       ";
            const string spacePadding = "  ";
            StringBuilder sb = new StringBuilder();
            //for (int row = Height - 1; row >= 0; row--)
            for (int row = 0; row < Height; row++)
            {
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    sb.Append(horizontalBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                //sb.Clear();
                //sb.Append(value: "|");
                //for (int column = 0; column < Width; column++)
                //{
                //    sb.Append(spaceBar); sb.Append(value: "|");
                //}
                //Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    if (wangTile.IsCollapsed)
                        sb.Append($"{spacePadding}{wangTile.CollapsedValue.edges[1].pathStyle.ToString()[0]},{wangTile.CollapsedValue.edges[1].edgeHeight.ToString()[0]}{spacePadding}");
                    else
                        sb.Append($"{spacePadding}X{spacePadding}");
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    sb.Append(spaceBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                //sb.Clear();
                //sb.Append(value: "|");
                //for (int column = 0; column < Width; column++)
                //{
                //    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode;
                //    sb.Append($"{spacePadding}{wangTile.CollapsedValue.edges[1].edgeHeight.ToString()[0]}{spacePadding}");
                //    sb.Append(value: "|");
                //}
                ////sb.Append(value: "|");
                //Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append($"{wangTile.CollapsedValue.edges[0].pathStyle.ToString()[0]}     {wangTile.CollapsedValue.edges[2].pathStyle.ToString()[0]}");
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append($"{wangTile.CollapsedValue.edges[0].edgeHeight.ToString()[0]}     {wangTile.CollapsedValue.edges[2].edgeHeight.ToString()[0]}");
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    sb.Append(spaceBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < Width; column++)
                {
                    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append($"{spacePadding}{wangTile.CollapsedValue.edges[3].pathStyle.ToString()[0]},{wangTile.CollapsedValue.edges[3].edgeHeight.ToString()[0]}{spacePadding}");
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                //sb.Append(value: "|");
                //for (int column = 0; column < Width; column++)
                //{
                //    sb.Append(spaceBar); sb.Append(value: "|");
                //}
                //Console.WriteLine(sb.ToString());
            }
            sb.Clear();
            sb.Append(value: "|");
            for (int column = 0; column < Width; column++)
            {
                sb.Append(horizontalBar); sb.Append(value: "|");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
