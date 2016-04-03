using ModernApplicationFramework.Caliburn.Collections;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public interface IUndoRedoManager
    {
        IObservableCollection<UndoRedoAction> RedoStack { get; }
        IObservableCollection<UndoRedoAction> UndoStack { get; }
        void Push(UndoRedoAction action);
        void Redo();

        void Undo();
    }
}