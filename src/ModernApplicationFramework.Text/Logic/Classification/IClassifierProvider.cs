using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassifierProvider
    {
        IClassifier GetClassifier(ITextBuffer textBuffer);
    }
}
