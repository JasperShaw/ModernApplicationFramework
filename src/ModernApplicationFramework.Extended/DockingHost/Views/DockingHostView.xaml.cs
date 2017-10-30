using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Extended.Core.Layout;
using ModernApplicationFramework.Extended.DockingHost.ViewModels;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.DockingHost.Views
{
    public partial class DockingHostView : IDockingHost
    {
        public DockingHostView()
        {
            InitializeComponent();
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

        private void DockingManager_OnLayoutUpdated(object sender, EventArgs e)
        {
            UpdateFloatingWindows();
        }
    }
}