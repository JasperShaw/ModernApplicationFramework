using System;
using System.Collections.Generic;
using System.IO;
using ModernApplicationFramework.Extended.Controls.DockingHost.Views;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <summary>
    /// A control that holds a <see cref="Docking.DockingManager"/>
    /// </summary>
    public interface IDockingHost
    {
        /// <summary>
        /// Occurs when one or more <see cref="ILayoutItem"/> is about to close.
        /// </summary>
        event EventHandler<LayoutItemsClosingEventArgs> LayoutItemsClosing;

        /// <summary>
        /// Occurs when one or more <see cref="ILayoutItem"/> was closed.
        /// </summary>
        event EventHandler<LayoutItemsClosedEventArgs> LayoutItemsClosed;

        /// <summary>
        /// Loads and applys a window layout.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="addToolCallback">The callback for adding the tools.</param>
        /// <param name="addDocumentCallback">The callback for adding layout items.</param>
        /// <param name="itemsState">The dictionary that contains all <see cref="ILayoutItemBase"/> to load.</param>
        void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<ILayoutItem> addDocumentCallback,
            Dictionary<string, ILayoutItemBase> itemsState);

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void SaveLayout(Stream stream);

        /// <summary>
        /// Updates all floating windows.
        /// </summary>
        void UpdateFloatingWindows();
    }
}