using System.Linq;
using System.Windows;

namespace ModernApplicationFramework.Controls
{
    /// <summary>
    /// A custom <see cref="System.Windows.Controls.ToolBarTray"/> control
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ToolBarTray" />
    public class ToolBarTray : System.Windows.Controls.ToolBarTray
    {
        public static readonly DependencyProperty IsMainToolBarTrayProperty = DependencyProperty.Register(
            "IsMainToolBarTray", typeof(bool), typeof(ToolBarTray), new PropertyMetadata(default(bool)));

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

        #region Override Methods 

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            ContainsVisibleToolBars = ToolBars.FirstOrDefault(t => t.Visibility == Visibility.Visible) != null;
            return size;
        }

        #endregion
    }
}