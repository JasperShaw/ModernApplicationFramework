using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Utilities.UndoRedoManager;
using Command = ModernApplicationFramework.Commands.Command;

namespace ModernApplicationFramework.MVVM.Controls
{
    public abstract class Document : LayoutItemBase, IDocument
    {
        private ICommand _closeCommand;

        private IUndoRedoManager _undoRedoManager;

        public ICommand RedoCommand => new Command(Redo, CanRedo);


        public ICommand UndoCommand => new Command(Undo, CanUndo);

        public IUndoRedoManager UndoRedoManager => _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager());

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true)); }
        }

        protected virtual bool CanRedo()
        {
            return UndoRedoManager.RedoStack.Any();
        }

        protected virtual bool CanUndo()
        {
            return UndoRedoManager.UndoStack.Any();
        }

        protected virtual async void Redo()
        {
            await Task.Run(() => UndoRedoManager.Redo());
        }


        protected virtual async void Undo()
        {
            await Task.Run(() => UndoRedoManager.Undo());
        }
    }
}