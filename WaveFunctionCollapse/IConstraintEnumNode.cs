// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintEnumNode<T> : IConstraintNode<T, T> where T : struct, System.Enum
    {
    }
}