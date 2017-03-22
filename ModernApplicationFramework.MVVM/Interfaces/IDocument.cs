using System.Windows.Input;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IDocument : ILayoutItem
    {
        ICommand RedoCommand { get; }

        ICommand SaveFileAsCommand { get; }

        ICommand SaveFileCommand { get; }

        ICommand UndoCommand { get; }

        IUndoRedoManager UndoRedoManager { get; }
    }
}