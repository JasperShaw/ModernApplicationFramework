using System.Windows.Controls;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class VerticalScrollBarContainerMargin : ContainerMargin
    {
        public VerticalScrollBarContainerMargin(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations,
            TextViewMarginState marginState)
            : base("VerticalScrollBarContainer", Orientation.Vertical, wpfTextViewHost, guardedOperations, marginState)
        {
        }

        internal static VerticalScrollBarContainerMargin Create(ITextViewHost wpfTextViewHost,
            GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            var barContainerMargin =
                new VerticalScrollBarContainerMargin(wpfTextViewHost, guardedOperations, marginState);
            barContainerMargin.Initialize();
            return barContainerMargin;
        }
    }
}