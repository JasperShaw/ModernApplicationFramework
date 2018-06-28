using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IMultiRedoCommand))]
    internal class MultiRedoCommand : CommandDefinitionCommand, IMultiRedoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public MultiRedoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.RedoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is int number))
                _watcher.UndoRedoManager.Redo(1);
            else
                _watcher.UndoRedoManager.Redo(number);
        }
    }
}