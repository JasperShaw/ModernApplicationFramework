using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    public class CustomizeControlsListBoxItem : ListBoxItem
    {
        public static DependencyProperty IsCheckedProperty;
        public static DependencyProperty IconProperty;
        private bool _contentLoaded;

        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        static CustomizeControlsListBoxItem()
        {
            IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(CustomizeControlsListBoxItem), new FrameworkPropertyMetadata());
            IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(CustomizeControlsListBoxItem), new FrameworkPropertyMetadata());
        }

        public CustomizeControlsListBoxItem()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this, new Uri("/ModernApplicationFramework;component/Themes/Generic/CustomizeControlsListBoxItem.xaml", UriKind.Relative));
        }
    }
}
