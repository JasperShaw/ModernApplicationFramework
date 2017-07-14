using System.Windows;
using System.Windows.Automation.Peers;
using ModernApplicationFramework.Controls.Windows;

namespace ModernApplicationFramework.Controls.AutomationPeer
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