using System.Collections.ObjectModel;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interfaces provides the view model for a page to manage toolbars
    /// </summary>
    internal interface IToolBarsPageViewModel : IScreen
    {
        /// <summary>
        /// A collection of all available toolbar data models
        /// </summary>
        ObservableCollection<CommandBarDefinitionBase> Toolbars { get; }

        /// <summary>
        /// The selected toolbar data model
        /// </summary>
        ToolbarDefinition SelectedToolbarDefinition { get; }

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