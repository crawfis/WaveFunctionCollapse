using CrawfisSoftware.Collections.Graph;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    internal static class TileSetCreation
    {
        const int Left = 0;
        const int Top = 1;
        const int Right = 2;
        const int Bottom = 3;

        public static List<TileState> CreateTileSet()
        {
            var tile = new TileState();
            tile.edges = new EdgeState[4];
            List<EdgeState> edgeStates = new List<EdgeState>(Enum.GetNames(typeof(EdgeHeight)).Length * Enum.GetNames(typeof(PathStyle)).Length);
            int j = 0;
            foreach (EdgeHeight edge in Enum.GetValues(typeof(EdgeHeight)))
            {
                foreach (PathStyle pathStyle in Enum.GetValues(typeof(PathStyle)))
                {
                    EdgeState candidate = new EdgeState() { edgeHeight = edge, pathStyle = pathStyle };
                    if (ValidEdgeState(candidate))
                        edgeStates.Add(candidate);
                    j++;
                }
            }

            var tileSet = new List<TileState>();
            foreach (var leftEdge in edgeStates)
            {
                foreach (var topEdge in edgeStates)
                {
                    foreach (var rightEdge in edgeStates)
                    {
                        foreach (var bottomEdge in edgeStates)
                        {
                            var tileCandidate = new TileState() { edges = new EdgeState[4] { leftEdge, topEdge, rightEdge, bottomEdge } };
                            if (ValidateTile(tileCandidate))
                            {
                                tileSet.Add(tileCandidate);
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"edgeStates.Count = {edgeStates.Count}");
            Console.WriteLine($"Possible edge states without filtering, j = {j}");
            Console.WriteLine($"Tile-set size = {tileSet.Count}");
            return tileSet;
        }

        private static bool ValidateTile(TileState tileCandidate)
        {
            var leftEdge = tileCandidate.edges[Left];
            var topEdge = tileCandidate.edges[Top];
            var rightEdge = tileCandidate.edges[Right];
            var bottomEdge = tileCandidate.edges[Bottom];
            Direction direction = Direction.None;
            direction |= leftEdge.pathStyle != PathStyle.None ? Direction.W : Direction.None;
            direction |= topEdge.pathStyle != PathStyle.None ? Direction.N : Direction.None;
            direction |= rightEdge.pathStyle != PathStyle.None ? Direction.E : Direction.None;
            direction |= bottomEdge.pathStyle != PathStyle.None ? Direction.S : Direction.None;

            int dirtCount = 0;
            int waterCount = 0;
            int overpassCount = 0;
            int roadCount = 0;
            int noPathCount = 0;
            int lowCount = 0;
            int midCount = 0;
            int highCount = 0;
            int impassibleCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (tileCandidate.edges[i].pathStyle == PathStyle.Dirt) dirtCount++;
                if (tileCandidate.edges[i].pathStyle == PathStyle.Water) waterCount++;
                if (tileCandidate.edges[i].pathStyle == PathStyle.Overpass) overpassCount++;
                if (tileCandidate.edges[i].pathStyle == PathStyle.Road) roadCount++;
                if (tileCandidate.edges[i].pathStyle == PathStyle.None) noPathCount++;
                if (tileCandidate.edges[i].edgeHeight == EdgeHeight.Low) lowCount++;
                if (tileCandidate.edges[i].edgeHeight == EdgeHeight.Mid) midCount++;
                if (tileCandidate.edges[i].edgeHeight == EdgeHeight.High) highCount++;
                if (tileCandidate.edges[i].edgeHeight == EdgeHeight.Impassible) impassibleCount++;
            }
            //int maxHeightCount = Math.Max(Math.Max(Math.Max(lowCount, midCount), highCount), impassibleCount);
            //if (overpassCount > 0 && maxHeightCount != 4) return false;
            //if (dirtCount > 2 || waterCount > 2 || overpassCount == 3 || overpassCount == 4) return false;
            //if (roadCount > 0 && roadCount + overpassCount + noPathCount != 4) return false;
            //if (dirtCount > 0 && dirtCount + roadCount + noPathCount != 4) return false;
            //if (waterCount == 1 && !direction.IsDeadEnd()) return false;
            //if (waterCount == 2)
            //if (overpassCount == 2 && (waterCount != 2 || roadCount != 2 || noPathCount !=2)) return false;
            bool smallSlope = false;
            if (impassibleCount > 0 && ((impassibleCount + highCount) == 4)) smallSlope = true;
            else if (highCount > 0 && (highCount + midCount == 4)) smallSlope = true;
            else if (midCount > 0 && (midCount + lowCount == 4)) smallSlope = true;
            if (!smallSlope) return false;

            return true;

            // Overpass paths can only be straights.
            if (direction.IsStraight() && leftEdge.pathStyle == PathStyle.Overpass && (!(rightEdge.pathStyle == PathStyle.Overpass) || (rightEdge.pathStyle == PathStyle.Road))) return false;
            if (direction.IsStraight() && rightEdge.pathStyle == PathStyle.Overpass && (!(leftEdge.pathStyle == PathStyle.Overpass) || (leftEdge.pathStyle == PathStyle.Road))) return false;
            if (direction.IsStraight() && topEdge.pathStyle == PathStyle.Overpass && (!(bottomEdge.pathStyle == PathStyle.Overpass) || (bottomEdge.pathStyle == PathStyle.Road))) return false;
            if (direction.IsStraight() && bottomEdge.pathStyle == PathStyle.Overpass && (!(topEdge.pathStyle == PathStyle.Overpass) || (topEdge.pathStyle == PathStyle.Road))) return false;
            // Dirt paths can only be straights, turns or dead-ends.
            //if (direction.IsCrossSection() && leftEdge.pathStyle == PathStyle.Dirt && topEdge.pathStyle == PathStyle.Dirt && rightEdge.pathStyle == PathStyle.Dirt && bottomEdge.pathStyle == PathStyle.Dirt) return false;
            // Water paths can only be straights or turns or dead-ends
            //if (direction.IsCrossSection() && leftEdge.pathStyle == PathStyle.Water && topEdge.pathStyle == PathStyle.Water && rightEdge.pathStyle == PathStyle.Water && bottomEdge.pathStyle == PathStyle.Water) return false;
            return true;
        }

        private static bool ValidEdgeState(EdgeState candidate)
        {
            //if ((candidate.pathStyle == PathStyle.None || candidate.pathStyle == PathStyle.Road) && candidate.edgeHeight == EdgeHeight.Low) return true;
            //return false;

            //if (candidate.pathStyle == PathStyle.Overpass) return false;
            //if (candidate.pathStyle == PathStyle.Dirt) return false;
            //if (candidate.edgeHeight == EdgeHeight.Impassible) return false;
            //if (candidate.edgeHeight == EdgeHeight.High) return false;
            //if (candidate.pathStyle == PathStyle.Water) return false;

            // No paths allowed on Impassible edges
            if (candidate.edgeHeight == EdgeHeight.Impassible && candidate.pathStyle != PathStyle.None) return false;
            // No overpasses on High or Impassible edges
            if (candidate.edgeHeight == EdgeHeight.High && candidate.pathStyle == PathStyle.Overpass) return false;
            if (candidate.edgeHeight == EdgeHeight.Impassible && candidate.pathStyle == PathStyle.Overpass) return false;
            // No dirt paths on a low edge height
            if (candidate.edgeHeight == EdgeHeight.Low && candidate.pathStyle == PathStyle.Dirt) return false;
            return true;
        }
    }
}