using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Controls.Utilities;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class QuickCustomizeButton : System.Windows.Controls.MenuItem
    {
        public static readonly DependencyProperty QuickCustomizeDataSourceProperty;


        private readonly System.Windows.Controls.MenuItem _customizeMenuItem;
        private readonly System.Windows.Controls.MenuItem _resetToolbarMenuItem;


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
            _customizeMenuItem = new System.Windows.Controls.MenuItem();
            _resetToolbarMenuItem = new System.Windows.Controls.MenuItem();
            _customizeMenuItem.Click += CustomizeMenuItem_Click;
            _customizeMenuItem.Header = "Customize...";
            _resetToolbarMenuItem.Click += ResetToolbarMenuItem_Click;
            _resetToolbarMenuItem.Header = "Reset Toolbar";


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
            System.Windows.Controls.MenuItem customizeMenuItem3 = _customizeMenuItem;
            compositeCollection.Add(customizeMenuItem3);
            System.Windows.Controls.MenuItem resetToolbarMenuItem3 = _resetToolbarMenuItem;
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
