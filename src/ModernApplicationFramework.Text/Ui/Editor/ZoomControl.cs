using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class ZoomControl : ComboBox
    {
        public static readonly DependencyProperty SelectedZoomLevelProperty = DependencyProperty.RegisterAttached(nameof(SelectedZoomLevel), typeof(double), typeof(ZoomControl));

        static ZoomControl()
        {
            //TODO: Style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomControl), new FrameworkPropertyMetadata(typeof(ZoomControl)));
        }

        public static void SetSelectedZoomLevel(DependencyObject control, double value)
        {
            control.SetValue(SelectedZoomLevelProperty, value);
        }

        public static double GetSelectedZoomLevel(DependencyObject control)
        {
            return (double)control.GetValue(SelectedZoomLevelProperty);
        }

        public double SelectedZoomLevel
        {
            get => GetSelectedZoomLevel(this);
            set => SetSelectedZoomLevel(this, value);
        }
    }
}
