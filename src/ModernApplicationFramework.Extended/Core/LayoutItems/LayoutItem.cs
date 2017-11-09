using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutItems
{
    public class LayoutItem : LayoutItemBase, ILayoutItem
    {
        private ICommand _closeCommand;

        private IUndoRedoManager _undoRedoManager;

        public override ICommand CloseCommand => _closeCommand ?? (_closeCommand = new DelegateCommand(Close));

        public ICommand RedoCommand => IoC.Get<RedoCommandDefinition>().Command;

        public ICommand UndoCommand => IoC.Get<UndoCommandDefinition>().Command;

        public IUndoRedoManager UndoRedoManager => _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager());

        protected virtual void PushUndoRedoManager(string sender, object value)
        {
            UndoRedoManager.Push(new UndoRedoAction(this, sender, value));
        }

        private void Close(object obj)
        {
            TryClose(null);
        }
    }
}