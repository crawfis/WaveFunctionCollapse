namespace CrawfisSoftware.WaveFunctionCollapse
{
    public interface IConstraintEnumNode<T> : IConstraintNode<T, T> where T : struct, System.Enum
    {
    }
}