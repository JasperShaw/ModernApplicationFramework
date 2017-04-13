using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class QuickCustomizeButton : System.Windows.Controls.MenuItem
    {
        public static readonly DependencyProperty QuickCustomizeDataSourceProperty;
        private static readonly Lazy<ResourceKey> boundMenuItemStyleKey;
        private static readonly IfElseConverter negBoolToVisConverter;


        private readonly System.Windows.Controls.MenuItem _customizeMenuItem;
        private readonly System.Windows.Controls.MenuItem _resetToolbarMenuItem;


        public static ResourceKey BoundMenuItemStyleKey => boundMenuItemStyleKey.Value;

        public new static ResourceKey SeparatorStyleKey { get; set; }

        public static ResourceKey CustomizeMenuItemStyleKey { get; set; }

        public static ResourceKey ResetToolbarMenuItemStyleKey { get; set; }


        public ItemCollection QuickCustomizeDataSource
        {
            get => (ItemCollection)GetValue(QuickCustomizeDataSourceProperty);
            set => SetValue(QuickCustomizeDataSourceProperty, value);
        }

        static QuickCustomizeButton()
        {
            QuickCustomizeDataSourceProperty = DependencyProperty.Register("QuickCustomizeDataSource", typeof(ItemCollection), typeof(QuickCustomizeButton), new PropertyMetadata(OnQuickCustomizeDataSourceChanged));
            boundMenuItemStyleKey = new Lazy<ResourceKey>(InitializeResourcekey);
            var ifElseConverter = new IfElseConverter
            {
                TrueValue = Visibility.Collapsed,
                FalseValue = Visibility.Visible
            };
            negBoolToVisConverter = ifElseConverter;
            SeparatorStyleKey = new ComponentResourceKey(typeof(QuickCustomizeButton), "SeparatorStyleKey");
            CustomizeMenuItemStyleKey = new ComponentResourceKey(typeof(QuickCustomizeButton), "CustomizeMenuItemStyleKey");
            ResetToolbarMenuItemStyleKey = new ComponentResourceKey(typeof(QuickCustomizeButton), "ResetToolbarMenuItemStyleKey");
        }

        private static ResourceKey InitializeResourcekey()
        {
            return new ComponentResourceKey(typeof(QuickCustomizeButton), "BoundMenuItemStyleKey");
        }


        public QuickCustomizeButton()
        {
            _customizeMenuItem = new System.Windows.Controls.MenuItem();
            _resetToolbarMenuItem = new System.Windows.Controls.MenuItem();
            _customizeMenuItem.Click += CustomizeMenuItem_Click;
            _resetToolbarMenuItem.Click += ResetToolbarMenuItem_Click;
            var binding = new Binding("DataContext.IsCustom")
            {
                Source = this,
                Converter = negBoolToVisConverter
            };
            _resetToolbarMenuItem.SetBinding(VisibilityProperty, binding);
            CreateMenu();

        }



        private void CreateMenu()
        {
            ItemsSource = null;
            var compositeCollection = new CompositeCollection();

            if (QuickCustomizeDataSource == null)
                return;

            foreach (var data in QuickCustomizeDataSource)
            {
                var item = data as CommandDefinitionButton;
                if (item == null)
                    continue;
                if (item.DataContext is CommandBarDefinitionBase definition && definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
                    continue;
                var mi = new MenuItem(item.DataContext as CommandBarDefinitionBase);
                compositeCollection.Add(mi);
            }

            var separator = new System.Windows.Controls.Separator();
            compositeCollection.Add(separator);
            compositeCollection.Add(_customizeMenuItem);
            compositeCollection.Add(_resetToolbarMenuItem);

            ItemsSource = compositeCollection;
        }

        private void ResetToolbarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature will be added sometime later when the framework can load/store current settings");
        }

        private void CustomizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Todo");
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsKeyboardFocusWithin)
                return;
            IsSubmenuOpen = false;
        }


        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement == null)
                throw new ArgumentException();
            if (Equals(frameworkElement, _customizeMenuItem))
                frameworkElement.SetResourceReference(StyleProperty, CustomizeMenuItemStyleKey);
            else if (Equals(frameworkElement, _resetToolbarMenuItem))
                frameworkElement.SetResourceReference(StyleProperty, ResetToolbarMenuItemStyleKey);
            else if (frameworkElement is System.Windows.Controls.Separator)
                frameworkElement.SetResourceReference(StyleProperty, SeparatorStyleKey);
            else
            {
                if (item is Control c && c.DataContext is CommandBarDefinitionBase)
                    frameworkElement.SetResourceReference(StyleProperty, BoundMenuItemStyleKey);
            }
            base.PrepareContainerForItemOverride(element, item);
        }


        private static void OnQuickCustomizeDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var quickCustomizeButton = (QuickCustomizeButton)d;
            quickCustomizeButton.QuickCustomizeDataSource = (ItemCollection) e.NewValue;

            quickCustomizeButton.CreateMenu();
        }
    }
}
