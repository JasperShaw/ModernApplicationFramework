namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ISmartIndentProvider
    {
        ISmartIndent CreateSmartIndent(ITextView textView);
    }
}