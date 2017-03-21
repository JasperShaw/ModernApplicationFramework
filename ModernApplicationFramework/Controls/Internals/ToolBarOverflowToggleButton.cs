using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class ToolBarOverflowToggleButton : ToggleButton
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                IsChecked = true;
                e.Handled = true;
            }
            else
                base.OnKeyDown(e);
        }
    }
}
