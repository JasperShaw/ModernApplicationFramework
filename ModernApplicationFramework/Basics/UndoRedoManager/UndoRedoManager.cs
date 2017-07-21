using System;
using System.Collections.Generic;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.UndoRedoManager
{
    /// <summary>
    /// An implementation of an <see cref="IUndoRedoManager" /> interface.
    /// </summary>
    /// <seealso cref="IUndoRedoManager" />
    /// <inheritdoc />
    /// <seealso cref="IUndoRedoManager" />
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly BindableCollection<UndoRedoAction> _redoStack;
        private readonly BindableCollection<UndoRedoAction> _undoStack;


        private bool _isChanging; //Make sure we do not end up in a loop inside the undo/redo manager

        /// <inheritdoc />
        /// <summary>
        /// The actions that can be redone
        /// </summary>
        public IObservableCollection<UndoRedoAction> RedoStack => _redoStack;

        /// <inheritdoc />
        /// <summary>
        /// The actions that can be undone
        /// </summary>
        public IObservableCollection<UndoRedoAction> UndoStack => _undoStack;


        /// <summary>
        /// Fires when an undo or redo action is about to start
        /// </summary>
        public event EventHandler ChangingBegin;

        /// <summary>
        /// Fires when an undo or redo action is completed
        /// </summary>
        public event EventHandler ChangingEnd;

        public UndoRedoManager()
        {
            _redoStack = new BindableCollection<UndoRedoAction>();
            _undoStack = new BindableCollection<UndoRedoAction>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Pushes an action to the <see cref="P:ModernApplicationFramework.Basics.UndoRedoManager.UndoRedoManager.UndoStack" /> and
        /// clears the <see cref="P:ModernApplicationFramework.Basics.UndoRedoManager.UndoRedoManager.RedoStack" /> if this method was executed outside of the manager
        /// </summary>
        /// <param name="action">The action that was performed and pushed to the <see cref="P:ModernApplicationFramework.Basics.UndoRedoManager.UndoRedoManager.UndoStack" /></param>
        public void Push(UndoRedoAction action)
        {
            Push(_undoStack, action);

            if (!_isChanging)
                _redoStack.Clear();
        }

        /// <inheritdoc />
        /// <summary>
        /// Redoes an amount of actions
        /// </summary>
        /// <param name="count">The number of actions to be redone</param>
        public void Redo(int count)
        {
            OnBegin();
            _isChanging = true;
            for (var i = 0; i < count; i++)
                Redo();
            _isChanging = false;
            OnEnd();
        }

        /// <inheritdoc />
        /// <summary>
        /// Undoes an amount of actions
        /// </summary>
        /// <param name="count">The number of actions to be undone</param>
        public void Undo(int count)
        {
            OnBegin();
            _isChanging = true;
            for (var i = 0; i < count; i++)
                Undo();
            _isChanging = false;
            OnEnd();
        }

        private static UndoRedoAction Pop(IList<UndoRedoAction> stack)
        {
            var item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return item;
        }

        private static void Push(ICollection<UndoRedoAction> stack, UndoRedoAction action)
        {
            stack.Add(action);
        }

        private void Redo()
        {
            var action = Pop(_redoStack);
            action.Execute();
            Push(_undoStack, action);

            //Needed as action.Execute() re-adds to the stack
            Pop(_undoStack);
        }

        private void Undo()
        {
            var action = Pop(_undoStack);
            action.Undo();
            Push(_redoStack, action);

            //Needed as action.Undo() re-adds to the stack
            Pop(_undoStack);
        }

        private void OnBegin()
        {
            var handler = ChangingBegin;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnEnd()
        {
            var handler = ChangingEnd;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}