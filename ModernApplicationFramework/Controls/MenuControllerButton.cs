using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class MenuControllerButton : System.Windows.Controls.Button
    {
        public ToolBar ParentToolBar => this.FindAncestor<ToolBar>();

        static MenuControllerButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuControllerButton),
                new PropertyMetadata(typeof(MenuControllerButton)));
        }

        protected override void OnClick()
        {
            this.FindAncestor<MenuController>().IsSubmenuOpen = false;
            base.OnClick();
        }

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            var ancestor = this.FindAncestor<MenuController>();
            ancestor.IsSubmenuOpen = true;
            Keyboard.Focus(ancestor);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            var ancestor = this.FindAncestor<System.Windows.Controls.Menu>();
            if (ancestor != null)
                Mouse.Capture(ancestor, CaptureMode.SubTree);
            base.OnLostMouseCapture(e);
        }
    }
}
