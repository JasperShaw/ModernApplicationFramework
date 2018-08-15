using System.Windows;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class IntraTextAdornment
    {
        public static readonly DependencyProperty IsSelected =
            DependencyProperty.RegisterAttached(nameof(IsSelected), typeof(bool), typeof(IntraTextAdornment));

        public static bool GetIsSelected(UIElement element)
        {
            return true.Equals(element.GetValue(IsSelected));
        }

        public static void SetIsSelected(UIElement element, bool isSelected)
        {
            element.SetValue(IsSelected, isSelected);
        }
    }
}