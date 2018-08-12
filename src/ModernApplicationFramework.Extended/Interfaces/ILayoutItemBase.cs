using System;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc cref="IScreen" />
    /// <summary>
    /// Basic interface that describes a view model used by the <see cref="IDockingHostViewModel"/>
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.IScreen" />
    /// <seealso cref="T:Caliburn.Micro.IViewAware" />
    public interface ILayoutItemBase : IScreen, IViewAware, IHasIconSource
    {
        /// <summary>
        /// Id that denotes the content
        /// </summary>
        string ContentId { get; }

        /// <summary>
        /// Id that denotes the instance
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets the tooltip string.
        /// </summary>
        string ToolTip { get; set; }

        /// <summary>
        /// Flag that indicates whether this layout item shall be reloaded on the next application start up
        /// </summary>
        bool ShouldReopenOnStart { get; }

        /// <summary>
        /// Indicates whether this layout item is selected
        /// </summary>
        bool IsSelected { get; set; }


        ///// <summary>
        ///// Gets or sets an <see cref="IContextMenuProvider"/>.
        ///// </summary>
        //IContextMenuProvider ContextMenuProvider { get; set; }

        /// <summary>
        /// The context menu definition identifier.
        /// </summary>
        Guid ContextMenuId { get; }


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


    


        /// <summary>
        /// Loads the state from a given <see cref="BinaryReader"/>  
        /// </summary>
        /// <param name="reader">The reader.</param>
        void LoadState(BinaryReader reader);

        /// <summary>
        /// Stores the current state to a <see cref="BinaryWriter"/>
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SaveState(BinaryWriter writer);
    }
}