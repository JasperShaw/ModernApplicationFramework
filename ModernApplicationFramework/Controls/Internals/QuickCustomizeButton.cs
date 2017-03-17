using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Controls.Utilities;

namespace ModernApplicationFramework.Controls.Internals
{



    internal class QuickCustomizeButton : MenuItem
    {
        public static readonly DependencyProperty QuickCustomizeDataSourceProperty;


        private readonly MenuItem customizeMenuItem;
        private readonly MenuItem resetToolbarMenuItem;
        private CollectionContainer boundItems;

        private ItemCollection quickCustomizeDataSource;


        public ItemCollection QuickCustomizeDataSource
        {
            get => (ItemCollection)GetValue(QuickCustomizeDataSourceProperty);
            set => SetValue(QuickCustomizeDataSourceProperty, value);
        }

        static QuickCustomizeButton()
        {
            QuickCustomizeDataSourceProperty = DependencyProperty.Register("QuickCustomizeDataSource", typeof(ItemCollection), typeof(QuickCustomizeButton), new PropertyMetadata(OnQuickCustomizeDataSourceChanged));
        }


        public QuickCustomizeButton()
        {
            customizeMenuItem = new MenuItem();
            resetToolbarMenuItem = new MenuItem();
            customizeMenuItem.Click += CustomizeMenuItem_Click;
            customizeMenuItem.Header = "Customize...";
            resetToolbarMenuItem.Click += ResetToolbarMenuItem_Click;
            resetToolbarMenuItem.Header = "Reset Toolbar";


            CreateMenu();

        }

        private void CreateMenu()
        {
            ItemsSource = null;
            CompositeCollection compositeCollection = new CompositeCollection();

            if (QuickCustomizeDataSource == null)
                return;

            foreach (var data in QuickCustomizeDataSource)
            {
                var item = data as ContentControl;

                if (item == null)
                    continue;


                var mi = new ContextMenuGlyphItem {Header = item.Name};
                if (item.IsVisible)
                    ContextMenuGlyphItemUtilities.SetCheckMark(mi);
                compositeCollection.Add(mi);
            }

           


            Separator separator3 = new Separator();


            compositeCollection.Add(separator3);
            MenuItem customizeMenuItem3 = customizeMenuItem;
            compositeCollection.Add(customizeMenuItem3);
            MenuItem resetToolbarMenuItem3 = resetToolbarMenuItem;
            compositeCollection.Add(resetToolbarMenuItem3);

            ItemsSource = compositeCollection;
        }

        private void ResetToolbarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Todo");
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

        private static void OnQuickCustomizeDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var quickCustomizeButton = (QuickCustomizeButton)d;
            quickCustomizeButton.QuickCustomizeDataSource = (ItemCollection) e.NewValue;
            quickCustomizeButton.CreateMenu();
        }
    }
}
