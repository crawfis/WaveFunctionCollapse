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