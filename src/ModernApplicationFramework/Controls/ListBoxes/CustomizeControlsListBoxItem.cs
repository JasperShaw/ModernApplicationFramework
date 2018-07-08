using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc cref="ListBoxItem" />
    /// <summary>
    /// The item control used by a <see cref="CustomizeControlsListBox" />. Supports themable icons
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ListBoxItem" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IThemableIconContainer" />
    public class CustomizeControlsListBoxItem : ListBoxItem
    {
        public static DependencyProperty IsCheckedProperty;
        public static DependencyProperty IconProperty;
        private bool _contentLoaded;

        /// <summary>
        /// Indicates whether the item is checked.
        /// </summary>
        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// The icon
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        static CustomizeControlsListBoxItem()
        {
            IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(CustomizeControlsListBoxItem), new FrameworkPropertyMetadata());
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(CustomizeControlsListBoxItem), new FrameworkPropertyMetadata());
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
