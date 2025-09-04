// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintNode<T, TChoices>
    {
        int Id { get; }
        bool IsCollapsed { get; }
        TChoices Possibilities { get; }
        T CollapsedValue { get; }
        int Entropy { get; }

        bool TryCollapseNode(System.Random random, out T collapsedValue);

        bool Reduce();

        void UpdateEntropy();
    }
}