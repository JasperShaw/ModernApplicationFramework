using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Controls
{
    /// <summary>
    /// Interaction logic for DockingHost.xaml
    /// </summary>
    public partial class DockingHost
    {
        public DockingHost()
        {
            InitializeComponent();
        }

        public DockingManager DockingManager => dockingManager;

        public void Connect(int connectionId, object target)
        {
        }

        private void DockingManager_OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            if (
                MessageBox.Show("Are you sure you want to close the document?", "AvalonDock Sample",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void LayoutElement_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot) sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine("ActiveContent-> {0}", activeContent);
            }
        }
    }
}