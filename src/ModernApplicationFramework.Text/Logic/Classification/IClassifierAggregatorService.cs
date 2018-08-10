using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassifierAggregatorService
    {
        IClassifier GetClassifier(ITextBuffer textBuffer);
    }
}