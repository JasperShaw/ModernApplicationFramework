namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    public interface IDropHandlerProvider
    {
        IDropHandler GetAssociatedDropHandler(ITextView wpfTextView);
    }
}