using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class OutliningCollapsedAdornmentControl : ContentControl
    {
        static OutliningCollapsedAdornmentControl()
        {
            //TODO: Style
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutliningCollapsedAdornmentControl),
                new FrameworkPropertyMetadata(typeof(OutliningCollapsedAdornmentControl)));
        }
    }
}