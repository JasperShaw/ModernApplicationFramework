using System.Windows;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    /// <summary>
    /// Interaktionslogik für CommandsPageView.xaml
    /// </summary>
    public partial class CommandsPageView : ICommandsPageView
    {
        public CommandsPageView()
        {
            InitializeComponent();
        }

        private void HandleStylingFlagsChange(object sender, RoutedEventArgs e)
        {

        }

        public DropDownDialogButton ModifySelectionButton => DropDownButton;
        public CustomizeControlsListBox CustomizeListBox => ControlsListBox;
    }

    public interface ICommandsPageView
    {
        DropDownDialogButton ModifySelectionButton { get; }

        CustomizeControlsListBox CustomizeListBox { get; }
    }
}
