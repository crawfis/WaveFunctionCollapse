// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using CrawfisSoftware.WaveFunctionCollapse;

namespace WFC_Tests.WangTilingTest
{
    internal class ConstrainedWangTile : WangTileConstraintNode<EdgeState, TileState>
    {
        public ConstrainedWangTile(int id, ISolver<TileState, IList<TileState>> solver, IList<TileState> initialPossibilities, IComparer<EdgeState> edgeComparer)
            : base(id, solver, initialPossibilities, edgeComparer)
        {
        }
        protected override bool CheckAdditionalConstraints()
        {
            bool reduced = false;
            // Interesting examples:
            // 1. Limited number of particular tiles.
            // 2. Limited number of tiles in a particular direction.
            // 3. Space out special tiles - larger neighborhood context.
            // 4. Check diagonal (corner) neighbors.
            return reduced;
        }
    }
}