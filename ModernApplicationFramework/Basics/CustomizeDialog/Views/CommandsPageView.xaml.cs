using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    /// <summary>
    /// Interaktionslogik für CommandsPageView.xaml
    /// </summary>
    public partial class CommandsPageView : UserControl
    {
        public CommandsPageView()
        {
            InitializeComponent();
        }

        private void ModifySelectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContextMenu dropDownMenu = ModifySelectionButton.DropDownMenu;
            dropDownMenu.IsOpen = true;
        }

        private void HandleStylingFlagsChange(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
