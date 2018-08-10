namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IScrollMapFactoryService
    {
        IScrollMap Create(ITextView textView);

        IScrollMap Create(ITextView textView, bool areElisionsExpanded);
    }
}