// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IWangTile<TEdge>
    {
        public enum EdgeLocation { Top, Right, Bottom, Left };
        TEdge GetEdge(EdgeLocation edge);
        TEdge this[int index] { get; }
    }
}