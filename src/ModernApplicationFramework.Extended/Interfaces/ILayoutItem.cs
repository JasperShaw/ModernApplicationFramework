using System.Windows.Input;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Simple Layout Item that supports undo/redo operations
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Extended.Interfaces.ILayoutItemBase" />
    public interface ILayoutItem : ILayoutItemBase
    {
        /// <summary>
        /// The redo command.
        /// </summary>
        ICommand RedoCommand { get; }

        /// <summary>
        /// The undo command.
        /// </summary>
        ICommand UndoCommand { get; }

        /// <summary>
        /// The undo redo manager for this Layout Item.
        /// </summary>
        IUndoRedoManager UndoRedoManager { get; }
    }
}