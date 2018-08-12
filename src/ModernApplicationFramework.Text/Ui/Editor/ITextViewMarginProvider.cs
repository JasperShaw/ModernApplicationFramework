namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewMarginProvider
    {
        ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer);
    }
}