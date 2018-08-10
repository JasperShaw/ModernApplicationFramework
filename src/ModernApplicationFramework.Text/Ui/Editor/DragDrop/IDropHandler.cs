namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    public interface IDropHandler
    {
        DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo);

        DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo);

        DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo);

        bool IsDropEnabled(DragDropInfo dragDropInfo);

        void HandleDragCanceled();
    }
}