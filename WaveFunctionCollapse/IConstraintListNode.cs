// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintListNode<T> : IConstraintNode<T, IList<T>>
    {
        // Works for Arrays and Lists.
    }
}