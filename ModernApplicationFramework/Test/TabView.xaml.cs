using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Test
{
    /// <summary>
    /// Interaktionslogik für TabView.xaml
    /// </summary>
    public partial class TabView
    {
        public TabView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolBarListBox.Focus();
            Loaded -= OnLoaded;

            ToolBarListBox.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("Content",
                    System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
