using System.Windows.Controls;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class VerticalScrollBarContainerMargin : ContainerMargin
    {
        public VerticalScrollBarContainerMargin(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
            : base("VerticalScrollBarContainer", Orientation.Vertical, wpfTextViewHost, guardedOperations, marginState)
        {
        }

        internal static VerticalScrollBarContainerMargin Create(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            var barContainerMargin = new VerticalScrollBarContainerMargin(wpfTextViewHost, guardedOperations, marginState);
            barContainerMargin.Initialize();
            return barContainerMargin;
        }
    }
}