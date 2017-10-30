using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc cref="ListBoxItem" />
    /// <summary>
    /// The item control used by a <see cref="CustomizeControlsListBox" />. Supports themable icons
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ListBoxItem" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IThemableIconContainer" />
    public class CustomizeControlsListBoxItem : ListBoxItem, IThemableIconContainer
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

        /// <inheritdoc />
        /// <summary>
        /// This is the original source to icon. Usually this is a resource containing a <see cref="T:System.Windows.Controls.Viewbox" /> element.
        /// </summary>
        public object IconSource { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// This is the icon converted from <see cref="P:ModernApplicationFramework.Controls.ListBoxes.CustomizeControlsListBoxItem.IconSource" />. Usually the object is a <see cref="T:System.Windows.Media.Imaging.BitmapSource" />.
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var definitionBase = DataContext as CommandBarDefinitionBase;
            if (string.IsNullOrEmpty(definitionBase?.CommandDefinition?.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = definitionBase.CommandDefinition.IconSource };
            IconSource = myResourceDictionary[definitionBase.CommandDefinition.IconId];
            this.SetThemedIcon();
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
