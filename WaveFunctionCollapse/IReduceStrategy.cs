using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IReduceStrategy<T, TChoices, N, M>
    {
        public int NumberOfPropagationCalls { get; set; } // Debug / Instrumentation
        public int NumberOfReduceCalls { get; set; } // Debug / Instrumentation
        bool PostCollapse(IEnumerable<int> changedNodeIndices, ISolver<T, TChoices, N, M> solver);
        bool RippleWave(int rippleNumber, ISolver<T, TChoices, N, M> solver);
    }
}