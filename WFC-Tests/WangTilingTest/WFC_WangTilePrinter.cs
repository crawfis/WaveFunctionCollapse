using System.Text;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    internal static class WFC_WangTilePrinter
    {
        public static void PrintTiling(ISolver<TileState, IList<TileState>> solver, int width, int height)
        {
            const string horizontalBar = "-------";
            const string spaceBar = "       ";
            const string spacePadding = "  ";
            StringBuilder sb = new StringBuilder();
            //for (int row = Height - 1; row >= 0; row--)
            for (int row = height - 1; row >= 0; row--)
            {
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    sb.Append(horizontalBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                //sb.Clear();
                //sb.Append(value: "|");
                //for (int column = 0; column < Width; column++)
                //{
                //    sb.Append(spaceBar); sb.Append(value: "|");
                //}
                //Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    var wangTile = solver.GetNode(row * width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append(TopData(spacePadding, wangTile));
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    sb.Append(spaceBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                //sb.Clear();
                //sb.Append(value: "|");
                //for (int column = 0; column < Width; column++)
                //{
                //    var wangTile = _solver.GetNode(row * Width + column) as WangTileConstraintNode;
                //    sb.Append($"{spacePadding}{wangTile.CollapsedValue.edges[1].edgeHeight.ToString()[0]}{spacePadding}");
                //    sb.Append(value: "|");
                //}
                ////sb.Append(value: "|");
                //Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    var wangTile = solver.GetNode(row * width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append(LeftRightPathStyle(wangTile));
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    var wangTile = solver.GetNode(row * width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append(LeftRightHeight(wangTile));
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    sb.Append(spaceBar); sb.Append(value: "|");
                }
                Console.WriteLine(sb.ToString());
                sb.Clear();
                sb.Append(value: "|");
                for (int column = 0; column < width; column++)
                {
                    var wangTile = solver.GetNode(row * width + column) as WangTileConstraintNode<EdgeState, TileState>;
                    sb.Append(BottomEdgeData(spacePadding, wangTile));
                    sb.Append(value: "|");
                }
                //sb.Append(value: "|");
                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
            sb.Clear();
            sb.Append(value: "|");
            for (int column = 0; column < width; column++)
            {
                sb.Append(horizontalBar); sb.Append(value: "|");
            }
            Console.WriteLine(sb.ToString());
        }

        private static string TopData(string spacePadding, WangTileConstraintNode<EdgeState, TileState> wangTile)
        {
            string tileData;
            if (wangTile.IsCollapsed)
                tileData = $"{spacePadding}{wangTile.CollapsedValue.edges[1].pathStyle.ToString()[0]},{wangTile.CollapsedValue.edges[1].edgeHeight.ToString()[0]}{spacePadding}";
            else
                tileData = $"{spacePadding} X {spacePadding}";
            return tileData;
        }

        private static string LeftRightHeight(WangTileConstraintNode<EdgeState, TileState>? wangTile)
        {
            string tileData;
            if (wangTile.IsCollapsed)
                tileData = $"{wangTile.CollapsedValue.edges[0].edgeHeight.ToString()[0]}     {wangTile.CollapsedValue.edges[2].edgeHeight.ToString()[0]}";
            else
                tileData = "X     X";
            return tileData;
        }

        private static string BottomEdgeData(string spacePadding, WangTileConstraintNode<EdgeState, TileState>? wangTile)
        {
            string tileData;
            if (wangTile.IsCollapsed)
                tileData = $"{spacePadding}{wangTile.CollapsedValue.edges[3].pathStyle.ToString()[0]},{wangTile.CollapsedValue.edges[3].edgeHeight.ToString()[0]}{spacePadding}";
            else
                tileData = $"{spacePadding} X {spacePadding}";
            return tileData;
        }

        private static string LeftRightPathStyle(WangTileConstraintNode<EdgeState, TileState> wangTile)
        {
            string tileData;
            if (wangTile.IsCollapsed)
                tileData = $"{wangTile.CollapsedValue.edges[0].pathStyle.ToString()[0]}     {wangTile.CollapsedValue.edges[2].pathStyle.ToString()[0]}";
            else
                tileData = "X     X";
            return tileData;
        }
    }
}