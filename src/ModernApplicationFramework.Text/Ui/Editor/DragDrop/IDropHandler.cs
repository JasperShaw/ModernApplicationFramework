namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    public interface IDropHandler
    {
        DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo);

        void HandleDragCanceled();

        DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo);
        DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo);

        bool IsDropEnabled(DragDropInfo dragDropInfo);
    }
}