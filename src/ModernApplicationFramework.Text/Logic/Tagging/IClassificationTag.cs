using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IClassificationTag : ITag
    {
        IClassificationType ClassificationType { get; }
    }
}