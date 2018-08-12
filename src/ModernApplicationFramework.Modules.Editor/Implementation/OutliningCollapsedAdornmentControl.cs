using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Modules.Editor.Implementation
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