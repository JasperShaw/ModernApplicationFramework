using System.Windows.Input;
using ModernApplicationFramework.Utilities.UndoRedoManager;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDocument : ILayoutItem
    {
        IUndoRedoManager UndoRedoManager { get; }

        ICommand UndoCommand { get; }

        ICommand RedoCommand { get; }
    }
}