namespace ModernApplicationFramework.TextEditor
{
    public interface IViewClassifierAggregatorService
    {
        IClassifier GetClassifier(ITextView textView);
    }
}