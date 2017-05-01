using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;

namespace ModernApplicationFramework.Interfaces
{
    public interface IUndoRedoManager
    {
        IObservableCollection<UndoRedoAction> RedoStack { get; }
        IObservableCollection<UndoRedoAction> UndoStack { get; }
        void Push(UndoRedoAction action);
        void Redo();
        void Undo();
        void Undo(int count);
    }
}