using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Extended.Controls.DockingHost.ViewModels;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;

namespace ModernApplicationFramework.Extended.Controls.DockingHost.Views
{
    public partial class DockingHostView : IInternalDockingHost
    {
        public event EventHandler<LayoutItemsClosingEventArgs> LayoutItemsClosing;
        public event EventHandler<LayoutItemsClosedEventArgs> LayoutItemsClosed;

        public IReadOnlyList<ILayoutItemBase> AllOpenLayoutItemsAsDocuments
        {
            get
            {
                return DockingManager.AllOpenDocuments.Where(x => x.Content is ILayoutItemBase).Select(x => x.Content)
                    .OfType<ILayoutItemBase>().ToList();
            }
        }

        public DockingHostView()
        {
            InitializeComponent();
            DockingManager.DocumentsClosing += DockingManager_DocumentsClosing;
            DockingManager.DocumentsClosed += DockingManager_DocumentsClosed;
        }

        public void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<ILayoutItem> addDocumentCallback,
            Dictionary<string, ILayoutItemBase> itemsState)
        {
            LayoutUtilities.LoadLayout(DockingManager, stream, addDocumentCallback, addToolCallback, itemsState);
        }

        public void SaveLayout(Stream stream)
        {
            LayoutUtilities.SaveLayout(DockingManager, stream);
        }

        public void UpdateFloatingWindows()
        {
            var mainWindow = Window.GetWindow(this);
            var mainWindowIcon = mainWindow?.Icon;
            var showFloatingWindowsInTaskbar = ((DockingHostViewModel) DataContext).ShowFloatingWindowsInTaskbar;
            foreach (var window in DockingManager.FloatingWindows)
            {
                var anchor = window.FindLogicalChildren<LayoutAnchorControl>();
                if (anchor != null)
                    continue;
                window.Icon = mainWindowIcon;
                window.ShowInTaskbar = showFloatingWindowsInTaskbar;
            }
        }

        public bool RaiseDocumentClosing(ILayoutItem layoutItem)
        {
            var args = new LayoutItemsClosingEventArgs(new List<ILayoutItem> {layoutItem});
            OnLayoutItemClosing(args);
            return args.Cancel;
        }

        private void DockingManager_DocumentsClosed(object sender, Docking.DocumentsClosedEventArgs e)
        {
            var layoutItems = new List<ILayoutItem>();
            foreach (var layoutDocument in e.Documents)
                if (layoutDocument.Content is ILayoutItem layoutItem)
                    layoutItems.Add(layoutItem);
            if (LayoutItemsClosed == null)
                return;
            var eventArgs = new LayoutItemsClosedEventArgs(layoutItems);
            OnLayoutItemsClosed(eventArgs);
        }

        private void DockingManager_DocumentsClosing(object sender, Docking.DocumentsClosingEventArgs e)
        {
            var layoutItems = new List<ILayoutItem>();

            foreach (var layoutDocument in e.Documents)
            {
                if (layoutDocument.Content is ILayoutItem layoutItem)
                    layoutItems.Add(layoutItem);
            }
            if (LayoutItemsClosing != null)
            {
                var eventArgs = new LayoutItemsClosingEventArgs(layoutItems);
                OnLayoutItemClosing(eventArgs);
                e.Cancel = eventArgs.Cancel;
            }
        }

        private void DockingManager_OnLayoutUpdated(object sender, EventArgs e)
        {
            UpdateFloatingWindows();
        }

        protected virtual void OnLayoutItemClosing(LayoutItemsClosingEventArgs e)
        {
            LayoutItemsClosing?.Invoke(this, e);
        }

        protected virtual void OnLayoutItemsClosed(LayoutItemsClosedEventArgs e)
        {
            LayoutItemsClosed?.Invoke(this, e);
        }
    }

    internal interface IInternalDockingHost : IDockingHost
    {
        bool RaiseDocumentClosing(ILayoutItem layoutItem);
    }

    public class LayoutItemsClosingEventArgs : CancelEventArgs
    {
        public IEnumerable<ILayoutItem> LayoutItems { get; }

        public LayoutItemsClosingEventArgs(IEnumerable<ILayoutItem> layoutItems)
        {
            LayoutItems = layoutItems;
        }
    }

    public class LayoutItemsClosedEventArgs : EventArgs
    {
        public IEnumerable<ILayoutItem> LayoutItems { get; }

        public LayoutItemsClosedEventArgs(IEnumerable<ILayoutItem> layoutItems)
        {
            LayoutItems = layoutItems;
        }
    }

    public class ToolClosedEventArgs : EventArgs
    {
        public ITool Tool { get; }

        public ToolClosedEventArgs(ITool tool)
        {
            Tool = tool;
        }
    }
}