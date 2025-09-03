# WaveFunctionCollapse

A small, extensible .NET library for experimenting with (and teaching) Wave Function Collapse style constraint solving. It focuses on clarity and pluggability over raw performance. The repository contains the core library (WaveFunctionCollapse) and a test / samples project (WFC-Tests) that demonstrates usage through two scenarios:

1. WFC_ColorTest – classic K-color (graph coloring) problem with an additional global constraint limiting the number of Red assignments.
2. WFC_MazeTest (aka WFC_WangTileTest) – a Wang-tile based terrain / maze style generation with edge matching, elevation (height) and path style constraints, plus an example of pruning choices to conform to a separately generated maze topology.

---
## Core Concepts

The library breaks the Wave Function Collapse process into composable pieces so you can replace or extend any part.

- Solver (ISolver<T,TChoices>, SolverWithOracles<T,TChoices>)
  Orchestrates collapsing nodes, invoking constraint propagation (Reduce Strategies) and raising events.
- Constraint Nodes (IConstraintNode<T,TChoices>)
  Encapsulate the state (domain of possibilities) of a single variable / cell.
  * Enum domain nodes: ConstraintEnumNodeWithOracles<T,N,M> (bit‑flag style enums).
  * BitArray domain nodes: ConstraintBitArrayNode (abstract base if you prefer raw bitsets).
  * List domain nodes (Wang tiles): WangTileConstraintNode<TEdge,TTile>.
- Node Factories (IConstraintNodeFactory<T,TChoices>)
  Create and configure nodes (e.g., ColorGridConstraintNodeFactory, WangTileConstraintNodeFactory).
- Reduce Strategies (IReduceStrategy<T,TChoices>)
  Decide how constraint propagation iterates:
  * UpdateAllReduceStrategy – brute force: re-run Reduce() on every node each ripple.
  * GraphReduceStrategy – targeted propagation using a graph & a queue (only neighbors / registered constraints are revisited).
- Oracles (ConstraintOracles)
  Pluggable delegates for collapsing (e.g., first option, random option) and default no‑op reducers.
- Entropy
  Each node exposes an Entropy value (e.g., number of remaining possibilities) that a NodeSelector can exploit.
- Events & Instrumentation
  Hooks for: OnNodeCollapseStarting, OnNodeCollapseEnded, OnNodeCollapsed, OnNodeChanged, RippleWaveCompleted, OnPropagateStarting, OnPropagateEnding plus counters (NumberOfPropagationCalls, NumberOfReduceCalls) on Reduce Strategies.

---
## High Level Flow

1. Build a graph or an implicit grid of indices.
2. Create a SolverWithOracles and pick a ReduceStrategy.
3. Create nodes via a factory (inject custom Collapse delegate and Reducer function if needed).
4. Initialize the solver with the node list.
5. Optionally set NodeSelector (heuristic for which node to collapse next).
6. Call TrySolve(seed, optionalLimit).
7. Read collapsed values (or partial state if you stopped early).

---
## Choices You Can Customize

- Domain Representation:
  * Enum flags (simple & fast bitwise membership) – use ConstraintEnumNodeWithOracles.
  * BitArray (arbitrary sized domains) – derive from ConstraintBitArrayNode.
  * Explicit object list – WangTileConstraintNode for tile matching problems.
- Collapse Strategy:
  * CollapseToFirstOption (deterministic) or CollapseToRandomOption (stochastic) or supply your own CollapseDelegate.
- Node Selection Heuristic:
  * Provide SolverWithOracles.NodeSelector: e.g., pick lowest entropy, random among lowest, scan order, etc.
- Propagation Strategy:
  * UpdateAllReduceStrategy (simple, easier to reason about, slower for large domains).
  * GraphReduceStrategy (incremental, adjacency driven).
- Constraint Expression:
  * Custom Reducer function (Func<int,T,T>) on enum nodes.
  * Override Reduce() / CheckAdditionalConstraints() for WangTileConstraintNode subclasses.
  * Compose multiple checks using CompositeConstraints if desired.
  * External pruning helpers (e.g., WFC_MazeTest.RestrictToMaze).
- Global Counters / Limited Resources:
  * Example: color test limits Red occurrences via stateful factory (ColorGridConstraintNodeFactory).
- Edge Compatibility:
  * Supply a custom IComparer<TEdge> for tile edge equivalence (e.g., compare only pathStyle, or style + elevation).
- Ripples & Termination:
  * Limit ripples by MaxRipples on strategies or stop after N collapses by passing numberToCollapse to TrySolve.

---
## Color (K‑Coloring) Example

Goal: Assign one of several colors to each grid cell so no adjacent cells share a color AND the total number of Red cells is capped.

Key Pieces:
- Enum Flags: Colors (Red | Green | Blue | ...)
- Node Factory: ColorGridConstraintNodeFactory sets Collapse delegate (random choice) and Reducer that:
  * Removes Red once the Red quota is exhausted.
  * Removes any color already uniquely fixed in a neighbor.
- Solver: SolverWithOracles<Colors,Colors>
- Reduce Strategy: (example) GraphReduceStrategy with grid adjacency or UpdateAllReduceStrategy for simplicity.

Sketch:
```csharp
var grid = new Grid<int,int>(width, height, null, null);
var solver = new SolverWithOracles<Colors, Colors>
{
    ReduceStrategy = new GraphReduceStrategy<Colors, Colors, int, int>(grid)
};
var factory = new ColorGridConstraintNodeFactory<int,int>(grid, solver, allColors, redLimit);
var nodes = Enumerable.Range(0, width*height).Select(i => factory.Create(i)).ToList();
solver.Initialize(nodes);
solver.TrySolve(seed);
```

Instrumentation (after solve):
- solver.ReduceStrategy.NumberOfPropagationCalls
- solver.ReduceStrategy.NumberOfReduceCalls

Console output (in sample) prints each cell’s final color; ambiguous (not yet fully collapsed) cells appear in gray.

---
## Wang Tile Maze Example

Demonstrates:
- Larger object domain: each TileState holds four edges (Left, Top, Right, Bottom) with (PathStyle, EdgeHeight).
- PathStyle: None, Road, Dirt, Water, or Overpass 
- EdgeHeight: Low, Mid, High, or Impassible
- Edge matching constraint enforced via WangTileConstraintNode.ReduceBasedOnEdges.
- Additional environmental pruning: WFC_MazeTest.RestrictToMaze ensures path connectivity lines up with a pre-computed maze (each tile’s path directions must equal the maze cell’s direction bitmask).
- Tile set is trimmed by specific rules (e.g, dirt to water is allowed, but other road types are not, likewise road to dirt is allowed others are not, overpasses but be on straights, etc.).
- Performance instrumentation comparing propagation.

Flow:
1. Create tile set (TileSetCreation.CreateTileSet()).
2. Pick GraphPropagation reducer (ReducerType.GraphPropagation) – uses GraphReduceStrategy.
3. Collapse tiles until solved or choices exhausted.
4. Count unique tiles used vs total set; print performance stats.

You can extend by subclassing WangTileConstraintNode (see ConstrainedWangTile) and overriding CheckAdditionalConstraints for:
- Limiting frequency of special tiles.
- Forcing spacing (distance) rules.
- Diagonal compatibility, biome grouping, etc.

---
## Events (Instrumentation Hooks)

Attach to these on SolverWithOracles:
- OnNodeCollapseStarting(int id, TChoices possibilities)
- OnNodeCollapseEnded(int id, T collapsedValue)
- OnNodeCollapsed(int id, T collapsedValue) – fires only if a collapse actually occurred.
- OnNodeChanged(int id) – whenever a Reduce() call changed a node’s domain.
- RippleWaveCompleted(int rippleIndex)
- OnPropagateStarting / OnPropagateEnding

Use these for logging, visualizers, progress bars, or adaptive heuristics.

---
## Writing a Custom Reducer (Enum Domain)

A Reducer: (nodeId, currentPossibilities) => newPossibilities.
Return the SAME value to signal no change; return a different value to trigger NodeUpdated + enqueue in propagation strategy.
Ensure determinism if repeatability matters (seeded randomness only inside Collapse delegates, not Reducers unless you accept non-determinism).

Example pattern (from color test):
```csharp
Colors GatherStateAndReduce(int nodeId, Colors colors)
{
    if (redCount <= 0) colors &= ~Colors.Red;        // global quota
    foreach (var neighbor in grid.Neighbors(nodeId))
    {
        var nVal = solver.GetNodeValues(neighbor);
        if (IsSingleton(nVal)) colors &= ~nVal;       // forbid neighbor’s unique color
    }
    return colors;
}
```

---
## GraphReduceStrategy vs UpdateAllReduceStrategy

| Aspect | GraphReduceStrategy | UpdateAllReduceStrategy |
|--------|---------------------|-------------------------|
| Propagation scope | Only neighbors / registered constraint nodes | Every node each ripple |
| Data needed | A graph (IIndexedGraph) and optional constraint map | None (just node list) |
| Performance | Scales better on sparse local constraints | Simplicity over speed |
| Parallelization | Uses Parallel.ForEach on initial PostCollapse set | (Commented example) could be parallelized |
| When to use | Large grids, local tile/color constraints | Small prototypes, teaching, debugging |

---
## Extending the Library

1. New Domain Type: Implement IConstraintNode<T,TChoices> or subclass ConstraintBitArrayNode / WangTileConstraintNode.
2. New Collapse Heuristic: Supply a new CollapseDelegate.<T>
3. Advanced Node Selection: Provide NodeSelector to pick next node (entropy tie-breakers, randomness, heuristics based on spatial patterns, etc.).
4. Composite Constraints: Build a CompositeConstraints container and call it inside your Reducer.
5. Additional Global State: Maintain counters or histograms inside your factory and reference them during reduction.
6. Custom Edge Logic: Implement IComparer<TEdge> for specialized matching.
7. Failure / Backtracking: (Not yet included) You could wrap SolverWithOracles and when a node hits zero possibilities, record state and attempt backtracking.

---
## Practical Tips

- Always minimize side effects in Reducers – deterministic, idempotent transformations make debugging easier.
- Keep Reducers fast; they are called very frequently.
- Track entropy updates if you implement adaptive NodeSelector heuristics (call node.UpdateEntropy()).
- For large tile sets, pre-index possibilities by edge signature to accelerate elimination (future optimization).
- Guard against empty Possibilities early (log and halt or implement recovery strategy).

---
## Version & Compatibility

- Targets: .NET Standard 2.1 (core library) and .NET 9 for tests (as configured).
- Some examples comment about BitOperations which requires .NET Core 3.0+; the code avoids a hard dependency by manual counting when necessary.

---
## Roadmap / Ideas

- Optional backtracking / contradiction recovery.
- Weighted randomness (bias collapse by frequency weights).
- Pluggable entropy-based NodeSelector implementations (priority queue).
- Incremental visualization hooks (e.g., emit snapshot per ripple).
- Performance benchmarks and profiling helpers.

---
## Getting Started Quickly

1. Clone repo & open solution.
2. Run WFC-Tests project.
3. Inspect console output for Color and Wang Tile examples.
4. Modify the solver configuration (switch reducer, change random seed, adjust constraints) and re-run.

---
## Dependencies

- CrawfisSoftware.Collections (included as project reference) - Needed for IGraph and Grid support. Heap is used in the test project (replace with PriorityQueue in .NET 6+ if desired or just a LINQ Sort and Take).
- CrawfisSoftware.Collections.IGraph (included as project reference) - Needed for IIndexedGraph interface and Grid class. Replace with your own grid/graph implementation if desired.
- CrawfisSoftware.Grid (included as project reference). Replace with your own grid implementation if desired.

---
## License

This project is licensed under the GNU General Public License v3.0 (GPL-3.0).

You may copy, distribute and modify the software under the terms of the GPL-3.0. You must disclose your source code and include the original copyright and license notice.

SPDX-License-Identifier: GPL-3.0-only

For full license text see the LICENSE file (add one with the standard GPL v3 text from https://www.gnu.org/licenses/gpl-3.0.txt).

---
## Contributing

Issues / PRs for: documentation clarifications, new strategy types, example scenarios, performance improvements.

Enjoy experimenting with constraint solving – and feel free to adapt the pieces to your own procedural generation projects.