using System;
using System.Collections.Generic;
using System.IO;
using ModernApplicationFramework.Extended.Controls.DockingHost.Views;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IDockingHost
    {
        event EventHandler<LayoutItemsClosingEventArgs> LayoutItemsClosing;

        event EventHandler<LayoutItemsClosedEventArgs> LayoutItemsClosed;

        void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<ILayoutItem> addDocumentCallback,
            Dictionary<string, ILayoutItemBase> itemsState);

        void SaveLayout(Stream stream);

        void UpdateFloatingWindows();
    }
}