using System.Collections.Generic;

namespace CrawfisSoftware.WaveFunctionCollapse
{
    public class WangTileConstraintNodeFactory<TEdge, TTile> : IConstraintNodeFactory<TTile, IList<TTile>> where TTile : IWangTile<TEdge>
    {
        private ISolver<TTile, IList<TTile>> _solver;
        private IList<TTile> _initialPossibilities;
        private IComparer<TEdge> _edgeComparer;
        private int _width;
        private int _height;

        public WangTileConstraintNodeFactory(ISolver<TTile, IList<TTile>> solver, IList<TTile> initialPossibilities, IComparer<TEdge> edgeComparer, int Width, int Height)
        {
            _solver = solver;
            _initialPossibilities = initialPossibilities;
            _edgeComparer = edgeComparer;
            _width = Width;
            _height = Height;
        }
        public IConstraintNode<TTile, IList<TTile>> Create(int nodeIndex)
        {
            var node = new WangTileConstraintNode<TEdge, TTile>(nodeIndex, _solver, _initialPossibilities, _edgeComparer);
            node.Width = _width;
            node.Height = _height;
            return node;
        }
    }
}