using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMenuHostViewModel
    {
        CommandBase.Command RightClickCommand { get; }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        /// </summary>
        bool AllowOpenToolBarContextMenu { get; set; }

        /// <summary>
        ///     Contains the MenuItems of the MenuHostControl
        /// </summary>
        ObservableCollection<MenuItem> Items { get; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        IMainWindowViewModel MainWindowViewModel { get; set; }

        ObservableCollection<MenuBarDefinition> MenuBars { get; }
        ObservableCollectionEx<MenuDefinition> MenuDefinitions { get; }
        ObservableCollectionEx<MenuItemGroupDefinition> MenuItemGroupDefinitions { get; }
        ObservableCollectionEx<MenuItemDefinition> MenuItemDefinitions { get; }
        ObservableCollection<CommandBarDefinitionBase> ExcludedMenuElementDefinitions { get; }


        /// <summary>
        /// Builds/Rebuilds the Menu
        /// </summary>
        void BuildMenu();
    }
}