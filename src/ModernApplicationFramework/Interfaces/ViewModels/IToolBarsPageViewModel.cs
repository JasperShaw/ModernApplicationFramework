using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interfaces provides the view model for a page to manage toolbars
    /// </summary>
    internal interface IToolBarsPageViewModel : ICustomizeDialogScreen
    {
        /// <summary>
        /// A collection of all available toolbar data models
        /// </summary>
        ObservableCollection<CommandBarDataSource> Toolbars { get; }

        /// <summary>
        /// The selected toolbar data model
        /// </summary>
        ToolBarDataSource SelectedToolBar { get; }

        /// <summary>
        /// The command to open a drop-down button
        /// </summary>
        Input.Command.Command DropDownClickCommand { get; }

        /// <summary>
        /// The command to delete the selected toolbar
        /// </summary>
        Input.Command.Command DeleteSelectedToolbarCommand { get; }

        /// <summary>
        /// The command to create a new toolbar
        /// </summary>
        Input.Command.Command CreateNewToolbarCommand { get; }
    }
}