using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ModernApplicationFramework.MVVM.Core.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.ViewModels;

namespace ModernApplicationFramework.MVVM.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class DockingHostView : IDockingHost
    {
        public DockingHostView()
        {
            InitializeComponent();
        }

        private void DockingManager_OnLayoutUpdated(object sender, EventArgs e)
        {
            UpdateFloatingWindows();
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
    }
}
