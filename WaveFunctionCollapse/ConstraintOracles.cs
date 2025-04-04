using System;
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public static class ConstraintOracles
    {
        public static N NoOpReducer<N>(int changedNodeIndex, N currentPossibilities)
        {
            // No-op by default
            return currentPossibilities;
        }

        public static bool CollapseToFirstOption<N>(N possibilities, System.Random random, out N collapsedValue) where N : struct, Enum
        {
            collapsedValue = default;
            foreach (N flag in Enum.GetValues(typeof(N)))
            {
                if (possibilities.HasFlag(flag))
                {
                    collapsedValue = (N)flag;
                    return true;
                }
            }
            return false;
        }

        public static bool CollapseToRandomOption<N>(N possibilities, System.Random random, out N collapsedValue) where N : struct, Enum
        {
            N singleOption = default;
            List<N> options = new List<N>();
            foreach (N flag in Enum.GetValues(typeof(N)))
            {
                if (possibilities.HasFlag(flag))
                {
                    options.Add(flag);
                }
            }
            if (options.Count > 0)
            {
                collapsedValue = options[random.Next(options.Count)];
                return true;
            }
            collapsedValue = singleOption;
            return false;
        }
    }
}