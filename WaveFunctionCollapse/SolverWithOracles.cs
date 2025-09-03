using System;
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    // Designed with the NMP design philosophy in mind: Not-My-Problem :-)
    // Removes any concerns about the following:
    // 1. How to select the next node to collapse
    // 2. How to propagate the changes to the other nodes
    // 3. How to reduce the possibilities of the other nodes
    // 4. How / when to update Entropy
    // 5. How to handle failure cases.
    // Adds several events to allow the user to hook into the process
    public class SolverWithOracles<T, TChoices> : ISolver<T, TChoices>
    {
        // Take an IIndexedGraph, and a state of the system, and iterate over the nodes to solve using Wave Function Collapse.
        private List<IConstraintNode<T, TChoices>> _nodes; // = new List<IConstraintNode<T, TChoices>>();
        private bool _stateChanged = false;

        public IEnumerable<IConstraintNode<T, TChoices>> Nodes { get { return _nodes; } }
        public IReduceStrategy<T, TChoices> ReduceStrategy { get; set; }
        public Func<int, ISolver<T, TChoices>, int> NodeSelector { get; set; }

        // Instrumentation
        public event Action<int, TChoices> OnNodeCollapseStarting;
        public event Action<int, T> OnNodeCollapseEnded;
        public event Action<int, T> OnNodeCollapsed;
        public event Action<int> OnNodeChanged;
        public event Action<int> RippleWaveCompleted;
        public event Action<ISolver<T, TChoices>> OnPropagateStarting;
        public event Action<ISolver<T, TChoices>> OnPropagateEnding;

        public TChoices GetNodeValues(int index)
        {
            return _nodes[index].Possibilities;
        }

        public IConstraintNode<T, TChoices> GetNode(int index)
        {
            return _nodes[index];
        }

        public void Initialize(List<IConstraintNode<T, TChoices>> nodes)
        {
            _nodes = nodes;
        }

        public bool TrySolve(int randomSeed, int numberToCollapse = int.MaxValue)
        {
            numberToCollapse = Math.Min(numberToCollapse, _nodes.Count);
            Random random = new Random(randomSeed);
            int i = 0;
            int nodeIndex = -1;  // = random.Next(nodes.Count);
            while (i < numberToCollapse && nodeIndex < _nodes.Count - 1)
            {
                // Get the next node to collapse
                // nodeIndex = random.Next(nodes.Count);
                nodeIndex++;
                int selectedNode = NodeSelector?.Invoke(nodeIndex, this) ?? nodeIndex;
                IConstraintNode<T, TChoices> node = _nodes[selectedNode];
                OnNodeCollapseStarting?.Invoke(node.Id, node.Possibilities);
                _stateChanged = false;
                if (node.TryCollapseNode(random, out T collapsedValue))
                {
                    OnNodeCollapsed?.Invoke(node.Id, collapsedValue);
                    _stateChanged = true;
                }
                OnNodeCollapseEnded?.Invoke(node.Id, collapsedValue);
                // Fire event that Propagate is starting
                OnPropagateStarting?.Invoke(this);
                if (_stateChanged && ReduceStrategy.PostCollapse(new List<int> { node.Id }, this))
                {
                    Propagate();
                }
                // Fire event that Propagate is completed
                OnPropagateEnding?.Invoke(this);
            }
            return i == _nodes.Count;
        }

        private void Propagate()
        {
            int rippleCount = 0;
            while (ReduceStrategy.RippleWave(rippleCount, this))
            {
                RippleWaveCompleted?.Invoke(rippleCount);
                rippleCount++;
            }
        }

        public void NodeUpdated(int id)
        {
            _stateChanged = true;
            OnNodeChanged?.Invoke(id);
        }
    }
}