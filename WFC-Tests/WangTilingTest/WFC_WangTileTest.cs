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
        private const int Height = 9;
        static Grid<int, int> _grid;
        static ISolver<TileState, IList<TileState>> _solver;
        private static List<TileState> _tileSet;
        private static int randomSeed = 1234;
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
                nodes.Add(node);
            }

            //solver.OnNodeCollapsed += Solver_OnNodeCollapsed;
            solver.Initialize(nodes);
            SetNodeSelector(solver);
            SetReducer(solver, reducerType);
            return solver;
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
            solver.ReduceStrategy = strategy;
            foreach (var node in solver.Nodes)
            {
                foreach (var neighbor in _grid.Neighbors(node.Id))
                {
                    strategy.AddGraphConstraint(node.Id, neighbor);
                }
            }
        }

        private static void SetNodeSelector(SolverWithOracles<TileState, IList<TileState>> solver)
        {
            Heap<IConstraintNode<TileState, IList<TileState>>> heapNodes = new Heap<IConstraintNode<TileState, IList<TileState>>>(new EntropyComparer<TileState, IList<TileState>>());
            foreach (var node in solver.Nodes)
            {
                heapNodes.Add(node);
            }
            solver.NodeSelector = (index, solver) => heapNodes.RemoveRoot().Id;
        }

        private static IConstraintNodeFactory<TileState, IList<TileState>> CreateNodeFactory(SolverWithOracles<TileState, IList<TileState>> solver, IList<TileState> initialChoices)
        {
            return new WangTileConstraintNodeFactory<EdgeState, TileState>(solver, initialChoices, new EdgeStateComparer(), Width, Height);
        }
    }
}
