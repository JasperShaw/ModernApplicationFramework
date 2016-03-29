using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.ViewModels;
using ModernApplicationFramework.MVVM.Core.Utilities;

namespace ModernApplicationFramework.MVVM.Views
{
    /// <summary>
    /// Interaktionslogik für DockingHost.xaml
    /// </summary>
    public partial class DockingHost : IDockingHost
    {
        public DockingHost()
        {
            InitializeComponent();
            DataContext = new DockingHostViewModel(this);
        }

        public void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<IDocument> addDocumentCallback, Dictionary<string, ILayoutItem> itemsState)
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
            var showFloatingWindowsInTaskbar = ((DockingHostViewModel)DataContext).ShowFloatingWindowsInTaskbar;
            foreach (var window in DockingManager.FloatingWindows)
            {
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
