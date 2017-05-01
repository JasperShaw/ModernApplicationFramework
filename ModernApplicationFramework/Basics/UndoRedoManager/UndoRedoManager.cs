using System.Collections.Generic;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.UndoRedoManager
{
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly BindableCollection<UndoRedoAction> _redoStack;
        private readonly BindableCollection<UndoRedoAction> _undoStack;


        private bool _isChanging; //Make sure we do not end up in a loop inside the undo/redo manager

        public IObservableCollection<UndoRedoAction> RedoStack => _redoStack;
        public IObservableCollection<UndoRedoAction> UndoStack => _undoStack;

        public UndoRedoManager()
        {
            _redoStack = new BindableCollection<UndoRedoAction>();
            _undoStack = new BindableCollection<UndoRedoAction>();
        }

        public void Push(UndoRedoAction action)
        {
            Push(_undoStack, action);

            if (!_isChanging)
                _redoStack.Clear();
        }

        public void Redo(int count)
        {
            _isChanging = true;
            for (var i = 0; i < count; i++)
                Redo();
            _isChanging = false;
        }

        public void Undo(int count)
        {
            _isChanging = true;
            for (var i = 0; i < count; i++)
                Undo();
            _isChanging = false;
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

            //Needed as action.Execute() readds to the stack
            Pop(_undoStack);
        }

        private void Undo()
        {
            var action = Pop(_undoStack);
            action.Undo();
            Push(_redoStack, action);

            //Needed as action.Undo() readds to the stack
            Pop(_undoStack);
        }
    }
}