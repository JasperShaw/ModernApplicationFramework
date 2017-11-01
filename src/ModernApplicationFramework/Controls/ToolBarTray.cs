using System.Linq;
using System.Windows;
using ModernApplicationFramework.Core.MenuModeHelper;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A custom <see cref="T:System.Windows.Controls.ToolBarTray" /> control
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ToolBarTray" />
    public class ToolBarTray : System.Windows.Controls.ToolBarTray
    {
        public static readonly DependencyProperty IsMainToolBarTrayProperty = DependencyProperty.Register(nameof(IsMainToolBarTray), typeof(bool), typeof(ToolBarTray), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsMainToolBarTrayChanged));

        public static readonly DependencyProperty ContainsVisibleToolBarsProperty = DependencyProperty.Register(
            "ContainsVisibleToolBars", typeof(bool), typeof(ToolBarTray), new PropertyMetadata(default(bool)));

        static ToolBarTray()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarTray),
                new FrameworkPropertyMetadata(typeof(ToolBarTray)));
        }

        public bool ContainsVisibleToolBars
        {
            get => (bool) GetValue(ContainsVisibleToolBarsProperty);
            set => SetValue(ContainsVisibleToolBarsProperty, value);
        }

        public bool IsMainToolBarTray
        {
            get => (bool) GetValue(IsMainToolBarTrayProperty);
            set => SetValue(IsMainToolBarTrayProperty, value);
        }

        /// <summary>
        /// Adds a tool bar.
        /// </summary>
        /// <param name="toolBar">The tool bar.</param>
        public void AddToolBar(ToolBar toolBar)
        {
            if (ToolBars.Contains(toolBar))
                return;
            ToolBars.Add(toolBar);
        }

        /// <summary>
        /// Checks whether a tool bar exists
        /// </summary>
        /// <param name="toolBar">The tool bar.</param>
        /// <returns></returns>
        public bool ToolBarExists(ToolBar toolBar)
        {
            return Enumerable.Contains(ToolBars, toolBar);
        }

        /// <summary>
        /// Removes a tool bar.
        /// </summary>
        /// <param name="toolBar">The tool bar.</param>
        public void RemoveToolBar(ToolBar toolBar)
        {
            if (!ToolBars.Contains(toolBar))
                return;
            ToolBars.Remove(toolBar);
        }

        /// <summary>
        /// Removes all toolbars.
        /// </summary>
        public void RemoveAllToolbars()
        {
            ToolBars.Clear();
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            ContainsVisibleToolBars = ToolBars.FirstOrDefault(t => t.Visibility == Visibility.Visible) != null;
            return size;
        }

        private static void OnIsMainToolBarTrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vsToolBarTray = (ToolBarTray)d;
            if (vsToolBarTray.IsMainToolBarTray)
                MenuModeHelper.RegisterMainToolBarTray(vsToolBarTray);
            else
                MenuModeHelper.UnregisterMainToolBarTray(vsToolBarTray);
        }
    }
}