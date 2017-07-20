using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// A service that keeps track of changes and supports undoing and redoing them
    /// </summary>
    public interface IUndoRedoManager
    {
        /// <summary>
        /// The actions that can be redone
        /// </summary>
        IObservableCollection<UndoRedoAction> RedoStack { get; }

        /// <summary>
        /// The actions that can be undone
        /// </summary>
        IObservableCollection<UndoRedoAction> UndoStack { get; }

        /// <summary>
        /// Fires when an undo or redo action is about to start
        /// </summary>
        event EventHandler ChangingBegin;

        /// <summary>
        /// Fires when an undo or redo action is completed
        /// </summary>
        event EventHandler ChangingEnd;

        /// <summary>
        /// Pushes an action to the <see cref="UndoStack"/> and 
        /// cleares the <see cref="RedoStack"/> if this method was executed outside of the manager
        /// </summary>
        /// <param name="action">The action that was performed and pushed to the <see cref="UndoStack"/></param>
        void Push(UndoRedoAction action);

        /// <summary>
        /// Redoes an amount of actions
        /// </summary>
        /// <param name="count">The number of actions to be redone</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        void Redo(int count);

        /// <summary>
        /// Undoes an amount of actions
        /// </summary>
        /// <param name="count">The number of actions to be undone</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        void Undo(int count);
    }
}