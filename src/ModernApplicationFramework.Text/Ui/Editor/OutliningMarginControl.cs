using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class OutliningMarginControl : ContentControl
    {
        static OutliningMarginControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutliningMarginControl), new FrameworkPropertyMetadata(typeof(OutliningMarginControl)));
        }
    }
}
