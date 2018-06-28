using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IMultiUndoCommand))]
    internal class MultiUndoCommand : CommandDefinitionCommand, IMultiUndoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public MultiUndoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is int number))
                _watcher.UndoRedoManager.Undo(1);
            else
                _watcher.UndoRedoManager.Undo(number);
        }
    }
}