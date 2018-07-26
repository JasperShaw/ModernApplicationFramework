namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationTag : ITag
    {
        IClassificationType ClassificationType { get; }
    }
}