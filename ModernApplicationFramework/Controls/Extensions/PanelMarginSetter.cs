using System.Windows;
using System.Windows.Controls;

//http://blogs.microsoft.co.il/eladkatz/2011/05/29/what-is-the-easiest-way-to-set-spacing-between-items-in-stackpanel/

namespace ModernApplicationFramework.Controls.Extensions
{
    /// <summary>
    /// Extension to set a fixed margin between stack panel items
    /// </summary>
    public class PanelMarginSetter
    {
        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin",
            typeof(Thickness), typeof(PanelMarginSetter),
            new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness) obj.GetValue(MarginProperty);
        }

        public static void PanelLoaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as Panel;

            if (panel == null)
                return;
            foreach (var child in panel.Children)
            {
                var fe = child as FrameworkElement;
                if (fe == null)
                    continue;
                fe.Margin = GetMargin(panel);
            }
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null)
                return;
            panel.Loaded += PanelLoaded;
        }
    }
}