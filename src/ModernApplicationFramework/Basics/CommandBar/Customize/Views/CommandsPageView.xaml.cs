using System.Windows;
using ModernApplicationFramework.Basics.CommandBar.Customize.ViewModels;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.Views
{
    /// <summary>
    ///     Interaktionslogik für CommandsPageView.xaml
    /// </summary>
    public partial class CommandsPageView : ICommandsPageView
    {
        //public DropDownDialogButton ModifySelectionButton => DropDownButton;
        public CustomizeControlsListBox CustomizeListBox => ControlsListBox;

        public CommandsPageView()
        {
            InitializeComponent();
        }

        private void HandleStylingFlagsChange(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CommandsPageViewModel;
            model?.HandleStylingFlagChangeCommand.Execute((CommandBarFlags) ((CheckedMenuItem) sender).Value);
        }

        private void HandleBeginGroup(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CommandsPageViewModel;
            model?.HandleAddOrRemoveGroupCommand.Execute(((CheckedMenuItem) sender).IsChecked);
        }

        private void HandleResetItem(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CommandsPageViewModel;
            model?.HandleResetItemCommand.Execute(null);
        }
    }

    public interface ICommandsPageView
    {
        // DropDownDialogButton ModifySelectionButton { get; }

        CustomizeControlsListBox CustomizeListBox { get; }
    }
}