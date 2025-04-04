namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintNodeFactory<T, TChoices>
    {
        IConstraintNode<T, TChoices> Create(int nodeIndex);
    }
}