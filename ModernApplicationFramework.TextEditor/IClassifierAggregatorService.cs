namespace ModernApplicationFramework.TextEditor
{
    public interface IClassifierAggregatorService
    {
        IClassifier GetClassifier(ITextBuffer textBuffer);
    }
}