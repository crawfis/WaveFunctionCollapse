using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class UpdateAllReduceStrategy<T, TChoices> : IReduceStrategy<T, TChoices>
    {
        public int NumberOfPropagationCalls { get; set; } = 0;
        public int NumberOfReduceCalls { get; set; } = 0;
        public int MaxRipples { get; set; } = int.MaxValue; // Instrumentation
        public bool PostCollapse(IEnumerable<int> _, ISolver<T, TChoices> solver)
        {
            return UpdateAll(solver);
        }

        public bool RippleWave(int rippleNumber, ISolver<T, TChoices> solver)
        {
            if (rippleNumber > MaxRipples) return false;
            return UpdateAll(solver);
        }

        private bool UpdateAll(ISolver<T, TChoices> solver)
        {
            bool changed = false;
            //Parallel.ForEach(solver.Nodes, node =>
            //{
            //    NumberOfPropagationCalls++;
            //    if (!node.IsCollapsed)
            //    {
            //        NumberOfReduceCalls++;
            //        if (node.Reduce())
            //        {
            //            changed = true;
            //        }
            //    }
            //});
            // Non-parallel version
            foreach (var node in solver.Nodes)
            {
                NumberOfReduceCalls++;
                if (node.Reduce())
                {
                    changed = true;
                }
            }
            return changed;
        }
    }
}