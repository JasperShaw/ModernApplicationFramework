using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Menu
{
    /// <inheritdoc />
    /// <summary>
    /// A custom button control used by a <see cref="T:ModernApplicationFramework.Controls.Menu.MenuController" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    public class MenuControllerButton : System.Windows.Controls.Button
    {
        /// <summary>
        /// The parent tool bar.
        /// </summary>
        public ToolBar ParentToolBar => this.FindAncestor<ToolBar>();

        static MenuControllerButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuControllerButton), new FrameworkPropertyMetadata(typeof(MenuControllerButton)));
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
