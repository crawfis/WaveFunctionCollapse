// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintNodeFactory<T, TChoices>
    {
        IConstraintNode<T, TChoices> Create(int nodeIndex);
    }
}