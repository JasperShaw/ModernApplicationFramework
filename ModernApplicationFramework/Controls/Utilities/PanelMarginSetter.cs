using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;


//http://blogs.microsoft.co.il/eladkatz/2011/05/29/what-is-the-easiest-way-to-set-spacing-between-items-in-stackpanel/
namespace ModernApplicationFramework.Controls.Utilities
{
    public class PanelMarginSetter
    {

        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness) obj.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin",
            typeof(Thickness), typeof(PanelMarginSetter),
            new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null)
                return;
            panel.Loaded += PanelLoaded;
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
    }
}
