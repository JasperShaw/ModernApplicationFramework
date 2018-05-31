using System;
using System.Windows;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Extended.Controls.DockingHost.ViewModels;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Controls.DockingHost.Views
{
    [TemplatePart(Name = "PART_DockingManager", Type = typeof(DockingManager))]
    public partial class DockingHostView : IDockingHost
    {
        public DockingManager DockingManager { get; }

        public DockingHostView()
        {
            InitializeComponent();
            DockingManager = dockingManager;
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