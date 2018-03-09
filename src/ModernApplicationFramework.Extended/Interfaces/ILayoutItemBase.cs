using System;
using System.IO;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc cref="IScreen" />
    /// <summary>
    /// Basic interface that describes a view model used by the <see cref="IDockingHostViewModel"/>
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.IScreen" />
    /// <seealso cref="T:Caliburn.Micro.IViewAware" />
    public interface ILayoutItemBase : IScreen, IViewAware
    {
        /// <summary>
        /// Id that denotes the content
        /// </summary>
        string ContentId { get; }

        /// <summary>
        /// Id that denotes the instance
        /// </summary>
        Guid Id { get; }


        string ToolTip { get; set; }

        /// <summary>
        /// Flag that indicates whether this layout item shall be reloaded on the next application start up
        /// </summary>
        bool ShouldReopenOnStart { get; }

        /// <summary>
        /// Indicates whether this layout item is selected
        /// </summary>
        bool IsSelected { get; set; }

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