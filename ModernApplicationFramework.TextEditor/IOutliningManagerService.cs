namespace ModernApplicationFramework.TextEditor
{
    public interface IOutliningManagerService
    {
        IOutliningManager GetOutliningManager(ITextView textView);
    }
}