using System;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    public class CustomizeControlsListBoxItem : ListBoxItem, IThemableIconContainer
    {
        public static DependencyProperty IsCheckedProperty;
        public static DependencyProperty IconProperty;
        private bool _contentLoaded;

        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public object IconSource { get; private set; }

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
