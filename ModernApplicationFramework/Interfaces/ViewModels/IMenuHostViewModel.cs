using System.Collections.ObjectModel;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMenuHostViewModel
    {
        MenuHostControl MenuHostControl { get; set; }

        Command RightClickCommand { get; }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        /// </summary>
        bool CanOpenToolBarContextMenu { get; set; }

        /// <summary>
        ///     Contains the MenuItems of the MenuHostControl
        /// </summary>
        ObservableCollection<MenuItem> Items { get; set; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        IMainWindowViewModel MainWindowViewModel { get; set; }

        /// <summary>
        ///     Create Menus
        /// </summary>
        /// <param name="creator"></param>
        void CreateMenu(IMenuCreator creator);
    }
}