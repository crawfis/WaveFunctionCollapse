namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IWangTile<TEdge>
    {
        public enum EdgeLocation { Top, Right, Bottom, Left };
        TEdge GetEdge(EdgeLocation edge);
        TEdge this[int index] { get; }
    }
}