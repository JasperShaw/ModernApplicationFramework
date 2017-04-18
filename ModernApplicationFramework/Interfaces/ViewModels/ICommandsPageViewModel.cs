using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface ICommandsPageViewModel : IScreen
    {
        ICommand HandleAddCommand { get; }
        ICommand HandleDeleteCommand { get; }
        ICommand HandleAddNewMenuCommand { get; }
        ICommand HandleMoveUpCommand { get; }
        ICommand HandleMoveDownCommand { get; }
        ICommand HandleAddOrRemoveGroupCommand { get; }
        ICommand HandleStylingFlagChangeCommand { get; }
        CommandBase.Command DropDownClickCommand { get; }

        IEnumerable<CommandBarDefinitionBase> CustomizableToolBars { get; set; }

        IEnumerable<CommandBarDefinitionBase> CustomizableMenuBars { get; set; }

        IEnumerable<CommandBarDefinitionBase> CustomizableContextMenus { get; set; }

        CommandBarDefinitionBase SelectedMenuItem { get; set; }

        CommandBarDefinitionBase SelectedToolBarItem { get; set; }

        CommandBarDefinitionBase SelectedContextMenuItem { get; set; }

        CustomizeRadioButtonOptions SelectedOption { get; set; }

        CommandBarItemDefinition SelectedListBoxDefinition { get; set; }

        IEnumerable<CommandBarItemDefinition> Items { get; set; }
    }
}