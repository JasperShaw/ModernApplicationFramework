using System.Windows;
using System.Windows.Automation.Peers;

namespace ModernApplicationFramework.Controls
{
    internal class MainWindowAutomationPeer : WindowAutomationPeer
    {
        public MainWindowAutomationPeer(Window owner) : base(owner) {}

        protected override string GetNameCore()
        {
            var mainWindow = Owner as MainWindow;
            if (mainWindow?.Title == null)
                return base.GetNameCore();
            return mainWindow.Title;
        }
    }
}