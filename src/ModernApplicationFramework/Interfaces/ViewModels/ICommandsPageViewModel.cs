using System.Collections.Generic;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interfaces provides the view model for a page to manage all command bar items
    /// </summary>
    public interface ICommandsPageViewModel : ICustomizeDialogScreen
    {
        /// <summary>
        /// The command to add a new command bar item
        /// </summary>
        ICommand HandleAddCommand { get; }

        /// <summary>
        /// The command to delete a new command bar item
        /// </summary>
        ICommand HandleDeleteCommand { get; }

        /// <summary>
        /// The command to add a new menu
        /// </summary>
        ICommand HandleAddNewMenuCommand { get; }

        /// <summary>
        /// The command to move a command bar item up
        /// </summary>
        ICommand HandleMoveUpCommand { get; }

        /// <summary>
        /// The command to move a command bar item down
        /// </summary>
        ICommand HandleMoveDownCommand { get; }

        /// <summary>
        /// The command to add or remove a group based on a parameter
        /// </summary>
        ICommand HandleAddOrRemoveGroupCommand { get; }

        /// <summary>
        /// The command to change the styling of command bar item
        /// </summary>
        ICommand HandleStylingFlagChangeCommand { get; }

        /// <summary>
        /// The command to open a drop-down button
        /// </summary>
        Input.Command.Command DropDownClickCommand { get; }

        /// <summary>
        /// A list of all tool bars that are customizable
        /// </summary>
        IEnumerable<CommandBarDefinitionBase> CustomizableToolBars { get; set; }

        /// <summary>
        /// A list of all menus that are customizable
        /// </summary>
        IEnumerable<CommandBarDefinitionBase> CustomizableMenuBars { get; set; }

        /// <summary>
        /// A list of all context menus that are customizable
        /// </summary>
        IEnumerable<CommandBarDefinitionBase> CustomizableContextMenus { get; set; }

        /// <summary>
        /// The currently selected menu item
        /// </summary>
        CommandBarDefinitionBase SelectedMenuItem { get; set; }

        /// <summary>
        /// The currently selected tool bar item
        /// </summary>
        CommandBarDefinitionBase SelectedToolBarItem { get; set; }

        /// <summary>
        /// he currently selected context menu item
        /// </summary>
        CommandBarDefinitionBase SelectedContextMenuItem { get; set; }

        /// <summary>
        /// The currently selected option
        /// </summary>
        CustomizeRadioButtonOptions SelectedOption { get; set; }

        /// <summary>
        /// The currently selected <see cref="CommandBarItemDefinition"/>
        /// </summary>
        CommandBarItemDefinition SelectedListBoxDefinition { get; set; }

        /// <summary>
        /// A list of all items displayed
        /// </summary>
        IEnumerable<CommandBarItemDefinition> Items { get; set; }
    }
}