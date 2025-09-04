// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using System;
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class CompositeConstraints<T, TChoices, N, M> where T : struct, Enum
    {
        private List<Func<IConstraintNode<T, TChoices>, int, T, bool>> _constraintFunctions = new List<Func<IConstraintNode<T, TChoices>, int, T, bool>>();

        public void AddConstraint(Func<IConstraintNode<T, TChoices>, int, T, bool> constraintFunc)
        {
            _constraintFunctions.Add(constraintFunc);
        }

        public bool CheckConstraints(IConstraintNode<T, TChoices> node, int nodeIndex, T neighborPossibilities)
        {
            foreach (var constraint in _constraintFunctions)
            {
                if (constraint(node, nodeIndex, neighborPossibilities))
                    return true;
            }
            return false;
        }
    }
}