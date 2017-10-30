using System.Windows.Input;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ILayoutItem : ILayoutItemBase
    {
        ICommand RedoCommand { get; }

        ICommand UndoCommand { get; }

        IUndoRedoManager UndoRedoManager { get; }
    }
}