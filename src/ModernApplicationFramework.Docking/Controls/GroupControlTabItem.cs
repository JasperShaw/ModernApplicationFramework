using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class GroupControlTabItem : TabItem
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(GroupControlTabItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)new CornerRadius(0.0)));

        public static CornerRadius GetCornerRadius(GroupControlTabItem element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(GroupControlTabItem element, CornerRadius value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(CornerRadiusProperty, value);
        }
    }
}