using System.Windows;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public static class CommandRouting
    {
        public static readonly DependencyProperty InterceptsCommandRouting =
            DependencyProperty.RegisterAttached(nameof(InterceptsCommandRouting), typeof(bool), typeof(CommandRouting),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static void SetInterceptsCommandRouting(DependencyObject element, bool value)
        {
            element.SetValue(InterceptsCommandRouting, value);
        }

        public static bool GetInterceptsCommandRouting(DependencyObject element)
        {
            return (bool)element.GetValue(InterceptsCommandRouting);
        }
    }
}