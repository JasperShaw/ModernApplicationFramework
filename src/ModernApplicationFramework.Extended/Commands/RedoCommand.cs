using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IRedoCommand))]
    internal class RedoCommand : CommandDefinitionCommand, IRedoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public RedoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher?.UndoRedoManager != null && _watcher.UndoRedoManager.RedoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            _watcher.UndoRedoManager.Redo(1);
        }
    }
}