using System.Collections.Generic;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.UndoRedoManager
{
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly BindableCollection<UndoRedoAction> _redoStack;
        private readonly BindableCollection<UndoRedoAction> _undoStack;

        public UndoRedoManager()
        {
            _redoStack = new BindableCollection<UndoRedoAction>();
            _undoStack = new BindableCollection<UndoRedoAction>();
        }

        public void Push(UndoRedoAction action)
        {
            Push(_undoStack, action);
        }

        public void Redo()
        {
            var action = Pop(_redoStack);
            action.Execute();
            Pop(_undoStack);
            Push(_undoStack, action);
        }

        public IObservableCollection<UndoRedoAction> RedoStack => _redoStack;

        public void Undo()
        {
            var action = Pop(_undoStack);
            action.Undo();
            Push(_redoStack, action);

            //Needed because otherwise we would be an odd loop
            if (action.HasPropertyChange)
                Pop(_undoStack);
        }

        public IObservableCollection<UndoRedoAction> UndoStack => _undoStack;

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
    }
}