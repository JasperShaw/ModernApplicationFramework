using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public interface IUndoRedoManager
    {
        IList<UndoRedoAction> RedoStack { get; }
        IList<UndoRedoAction> UndoStack { get; }

        void Undo();
        void Redo();
        void Push(UndoRedoAction action);
    }
}
