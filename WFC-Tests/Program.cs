// SPDX-License-Identifier: GPL-3.0-only
// Copyright (c) 2025 Dr. Roger Crawfis, CrawfisSoftware
using CrawfisSoftware.WaveFunctionCollapse;

using System.Numerics;

namespace CrawfisSoftware.WaveFunctionCollapse.Tests
{
    internal static class WFC_Tests
    {
        static void Main(string[] args)
        {
            WFC_ColorTest.ColorTest();
            WFC_WangTileTest.MazeTest();
        }
    }
}