// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.Maze;
using CrawfisSoftware.Maze.PerfectMazes;
using CrawfisSoftware.WaveFunctionCollapse;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WFC_Tests.WangTilingTest;

namespace WFC_Tests.MazeTest
{
    internal static class WFC_MazeTest
    {
        public static Maze<int, int> CreateMaze(int width, int height)
        {
            var builder = new MazeBuilder<int, int>(width, height);
            // Freeze the top row.
            for (int i = 0; i < width; i++)
                builder.RemoveUndefine(height - 1, i);
            builder.RecursiveBacktracking(0, true);
            builder.RemoveDeadEnds();
            builder.RemoveDeadEnds();
            builder.RemoveDeadEnds();
            builder.RemoveDeadEnds();
            builder.RemoveDeadEnds();
            builder.MergeDeadEndsRandomly();
            builder.MergeDeadEndsRandomly();
            builder.MergeDeadEndsRandomly();
            builder.RemoveUndefines();
            return builder.GetMaze();
        }
        public static void RestrictToMaze(Maze<int, int> maze, WangTileConstraintNode<EdgeState, TileState> node)
        {
            int row = node.Id / maze.Width;
            int column = node.Id % maze.Width;
            Direction direction = maze.GetDirection(column, row);
            for (int i = node.Possibilities.Count - 1; i >= 0; i--)
            {
                var possibility = node.Possibilities[i];
                // Check if the possibility is valid in the maze context.
                if (!IsValidInMaze(possibility, direction))
                {
                    node.Possibilities.RemoveAt(i);
                }
            }
        }

        private static bool IsValidInMaze(TileState possibility, Direction direction)
        {
            Direction nodeDirection = Direction.None;
            if (possibility.edges[WFC_WangTileTest.Left].pathStyle != PathStyle.None)
                nodeDirection |= Direction.W;
            if (possibility.edges[WFC_WangTileTest.Top].pathStyle != PathStyle.None)
                nodeDirection |= Direction.N;
            if (possibility.edges[WFC_WangTileTest.Right].pathStyle != PathStyle.None)
                nodeDirection |= Direction.E;
            if (possibility.edges[WFC_WangTileTest.Bottom].pathStyle != PathStyle.None)
                nodeDirection |= Direction.S;
            // Check if the possibility is valid in the maze context.
            return direction == nodeDirection;
        }
    }
}