using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class HorizontalScrollBarContainerMargin : ContainerMargin
    {
        public HorizontalScrollBarContainerMargin(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations,
            TextViewMarginState marginState)
            : base("HorizontalScrollBarContainer", Orientation.Horizontal, wpfTextViewHost, guardedOperations,
                marginState)
        {
            VerticalAlignment = VerticalAlignment.Bottom;
        }

        internal static HorizontalScrollBarContainerMargin Create(ITextViewHost wpfTextViewHost,
            GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            var barContainerMargin =
                new HorizontalScrollBarContainerMargin(wpfTextViewHost, guardedOperations, marginState);
            barContainerMargin.Initialize();
            return barContainerMargin;
        }
    }
}