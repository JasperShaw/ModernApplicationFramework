using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IUndoCommand))]
    internal class UndoCommand : CommandDefinitionCommand, IUndoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public UndoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher?.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            _watcher.UndoRedoManager.Undo(1);
        }
    }
}