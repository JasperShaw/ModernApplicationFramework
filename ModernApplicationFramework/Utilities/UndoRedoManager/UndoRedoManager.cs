using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public class UndoRedoManager : IUndoRedoManager
    {
        public IList<UndoRedoAction> RedoStack { get; }
        public IList<UndoRedoAction> UndoStack { get; }
        public void Undo()
        {
            throw new NotImplementedException();
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Push(UndoRedoAction action)
        {
            throw new NotImplementedException();
        }
    }
}
