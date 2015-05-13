using System.Linq;
using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarTray : System.Windows.Controls.ToolBarTray
    {
        public static readonly DependencyProperty IsMainToolBarProperty = DependencyProperty.Register(
            "IsMainToolBarTray", typeof (bool), typeof (ToolBarTray), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ContainsVisibleToolBarsProperty = DependencyProperty.Register(
            "ContainsVisibleToolBars", typeof (bool), typeof (ToolBarTray), new PropertyMetadata(default(bool)));

        public bool ContainsVisibleToolBars
        {
            get { return (bool) GetValue(ContainsVisibleToolBarsProperty); }
            set { SetValue(ContainsVisibleToolBarsProperty, value); }
        }

        public bool IsMainToolBar
        {
            get { return (bool) GetValue(IsMainToolBarProperty); }
            set { SetValue(IsMainToolBarProperty, value); }
        }

        static ToolBarTray()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarTray), new FrameworkPropertyMetadata(typeof(ToolBarTray)));
        }

        #region Override Methods 

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            ContainsVisibleToolBars = ToolBars.FirstOrDefault(t => t.Visibility == Visibility.Visible) != null;
            return size;
        }

        #endregion

        public void ShowToolBar(ToolBar toolBar)
        {
            if (ToolBars.Contains(toolBar))
                return;
            ToolBars.Add(toolBar);
        }

        public void RemoveToolBar(ToolBar toolBar)
        {
            if (!ToolBars.Contains(toolBar))
                return;
            ToolBars.Remove(toolBar);
        }
    }
}
