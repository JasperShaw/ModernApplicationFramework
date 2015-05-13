using System.Windows;
using System.Windows.Automation.Peers;

namespace ModernApplicationFramework.Controls
{
    internal class MainWindowTitleBarAutomationPeer : FrameworkElementAutomationPeer
    {
        public MainWindowTitleBarAutomationPeer(FrameworkElement owner) : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TitleBar;
        }

        protected override string GetNameCore()
        {
            var presentationSource = PresentationSource.FromVisual(Owner);
            if (presentationSource == null)
                return "TitleBar";
            var window = presentationSource.RootVisual as Window;
            return window != null ? window.Title : "TitleBar";
        }

        protected override string GetAutomationIdCore()
        {
            return "TitleBar";
        }
    }
}
