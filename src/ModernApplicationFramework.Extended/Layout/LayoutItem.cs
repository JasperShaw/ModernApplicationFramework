using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutItem : LayoutItemBase, ILayoutItem
    {
        private IUndoRedoManager _undoRedoManager;

        public ICommand RedoCommand => IoC.Get<RedoCommandDefinition>().Command;

        public ICommand UndoCommand => IoC.Get<UndoCommandDefinition>().Command;

        public IUndoRedoManager UndoRedoManager => _undoRedoManager ?? (_undoRedoManager = new Basics.UndoRedoManager.UndoRedoManager());

        protected virtual void PushUndoRedoManager(string sender, object value)
        {
            UndoRedoManager.Push(new UndoRedoAction(this, sender, value));
        }
    }
}