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
        }

        private void ModifySelectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            var d = (ToolbarDefinition) ToolBarListBox.SelectedItem;
            ContextMenu dropDownMenu = ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = d;
            dropDownMenu.IsOpen = true;
        }

        private void HandleToolbarNameChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}
