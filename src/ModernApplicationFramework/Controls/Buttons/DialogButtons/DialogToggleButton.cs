using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernApplicationFramework.Controls.Buttons.DialogButtons
{
    public class DialogToggleButton : ToggleButton
    {
        private ToolTip _keyboardFocusToolTip;
        private bool _keyboardFocusToolTipEnabled;

        static DialogToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogToggleButton), new FrameworkPropertyMetadata(typeof(DialogToggleButton)));
        }

        protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_keyboardFocusToolTip == null)
            {
                if (ToolTip is string str)
                {
                    var toolTip = new ToolTip {Content = str};
                    _keyboardFocusToolTip = toolTip;
                    _keyboardFocusToolTip.PlacementTarget = this;
                    _keyboardFocusToolTip.Placement = PlacementMode.Bottom;
                    _keyboardFocusToolTip.VerticalOffset = 3.0;
                    _keyboardFocusToolTip.HorizontalOffset = ActualWidth / 2.0;
                }
            }
            if (_keyboardFocusToolTip != null)
            {
                if ((bool) e.NewValue)
                {
                    if (_keyboardFocusToolTipEnabled)
                    {
                        _keyboardFocusToolTip.IsOpen = true;
                        _keyboardFocusToolTip.StaysOpen = false;
                    }
                }
                else
                {
                    _keyboardFocusToolTip.StaysOpen = true;
                    _keyboardFocusToolTip.IsOpen = false;
                }
            }
            base.OnIsKeyboardFocusedChanged(e);
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            _keyboardFocusToolTipEnabled = false;
            base.OnToolTipOpening(e);
        }

        protected override void OnToolTipClosing(ToolTipEventArgs e)
        {
            _keyboardFocusToolTipEnabled = true;
            base.OnToolTipClosing(e);
        }
    }
}
