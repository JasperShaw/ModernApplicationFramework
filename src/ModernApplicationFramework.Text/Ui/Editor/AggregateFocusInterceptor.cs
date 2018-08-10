using System.Windows;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class AggregateFocusInterceptor
    {
        public static readonly DependencyProperty InterceptsAggregateFocus = DependencyProperty.RegisterAttached(
            nameof(InterceptsAggregateFocus), typeof(bool), typeof(AggregateFocusInterceptor),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.Inherits));

        public static void SetInterceptsAggregateFocus(DependencyObject element, bool value)
        {
            element.SetValue(InterceptsAggregateFocus, value);
        }

        public static bool GetInterceptsAggregateFocus(DependencyObject element)
        {
            return (bool)element.GetValue(InterceptsAggregateFocus);
        }
    }
}