namespace ModernApplicationFramework.TextEditor
{
    public interface ISmartIndentProvider
    {
        ISmartIndent CreateSmartIndent(ITextView textView);
    }
}