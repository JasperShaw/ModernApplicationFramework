using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls.Internals
{
    internal sealed class SelectionDelegatingCheckBox : System.Windows.Controls.CheckBox
    {
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.FindAncestor<CheckableListBoxItem>().Focus();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled)
                return;
            e.Handled = false;
        }
    }
}