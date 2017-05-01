using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;

namespace ModernApplicationFramework.Interfaces
{
    public interface IUndoRedoManager
    {
        IObservableCollection<UndoRedoAction> RedoStack { get; }
        IObservableCollection<UndoRedoAction> UndoStack { get; }

        event EventHandler ChangingBegin;
        event EventHandler ChangingEnd;

        void Push(UndoRedoAction action);
        void Redo(int count);
        void Undo(int count);
    }
}