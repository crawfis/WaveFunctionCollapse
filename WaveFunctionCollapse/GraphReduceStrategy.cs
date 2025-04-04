using CrawfisSoftware.Collections.Graph;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class GraphReduceStrategy<T, TChoices, N, M> : IReduceStrategy<T, TChoices, N, M>
    {
        private IIndexedGraph<N, M> _graph;
        public int NumberOfPropagationCalls { get; set; } = 0;
        public int NumberOfReduceCalls { get; set; } = 0;

        private Queue<int> _updateStack = new Queue<int>();
        Dictionary<int, List<int>> _nodeToConstraintMap = new Dictionary<int, List<int>>();

        public int MaxRipples { get; set; } = 10000000; // ten million

        public GraphReduceStrategy(IIndexedGraph<N, M> graph)
        {
            _graph = graph;
        }
        public void AddGraphConstraint(int nodeId, int constraintId)
        {
            if (!_nodeToConstraintMap.TryGetValue(nodeId, out List<int> nodeList))
            {
                nodeList = new List<int>();
                _nodeToConstraintMap.Add(nodeId, nodeList);
            }
            nodeList.Add(constraintId);
        }

        public bool PostCollapse(IEnumerable<int> changedNodeIndices, ISolver<T, TChoices, N, M> solver)
        {
            bool changed = false;
            foreach (var changedId in changedNodeIndices)
            {
                if (!_nodeToConstraintMap.TryGetValue(changedId, out List<int> nodeList)) continue;
                if (nodeList == null || nodeList.Count == 0) continue;
                Parallel.ForEach(nodeList, nodeId =>
                {
                    NumberOfPropagationCalls++;
                    var node = solver.GetNode(nodeId);
                    if (!node.IsCollapsed)
                    {
                        if (node.Reduce())
                        {
                            NodeUpdated(node.Id);
                            changed = true;
                            NumberOfReduceCalls++;
                        }
                    }
                });
            }
            return changed;
        }
        public bool RippleWave(int rippleNumber, ISolver<T, TChoices, N, M> solver)
        {
            bool changed = false;
            if (rippleNumber > MaxRipples) return false;
            var nodeList = _updateStack.ToArray();
            _updateStack.Clear();
            for (int i = 0; i < nodeList.Length; i++)
            {
                NumberOfPropagationCalls++;
                int nodeId = nodeList[i];
                foreach (var neighborId in _graph.Neighbors(nodeId))
                {
                    var node = solver.GetNode(neighborId);
                    if (node.IsCollapsed) continue;
                    if (node.Reduce())
                    {
                        NodeUpdated(node.Id);
                        changed = true;
                        NumberOfReduceCalls++;
                    }
                }
            }
            return changed;
        }
        private void NodeUpdated(int nodeId)
        {
            if (_updateStack.Contains(nodeId))
            {
                return;
            }
            _updateStack.Enqueue(nodeId);
        }
    }
}