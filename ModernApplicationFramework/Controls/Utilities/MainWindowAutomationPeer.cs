using System.Windows;
using System.Windows.Automation.Peers;

namespace ModernApplicationFramework.Controls.Utilities
{
    internal class MainWindowAutomationPeer : WindowAutomationPeer
    {
        public MainWindowAutomationPeer(Window owner) : base(owner) {}

        protected override string GetNameCore()
        {
            var mainWindow = Owner as MainWindow;
            return mainWindow?.Title ?? base.GetNameCore();
        }
    }
}