using System.Collections.ObjectModel;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.ViewModels
{
    public interface IMenuHostViewModel
    {
        MenuHostControl MenuHostControl { get; }

        /// <summary>
        /// Tells if you can open the ToolbarHostContextMenu
        /// </summary>
        bool CanOpenToolBarContextMenu { get; set; }

        /// <summary>
        /// Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        IMainWindowViewModel MainWindowViewModel { get; set; }

        /// <summary>
        /// Contains the MenuItems of the MenuHostControl
        /// </summary>
        ObservableCollection<MenuItem> Items { get; set; }

        Command RightClickCommand { get; }

        /// <summary>
        /// Create Menus 
        /// </summary>
        /// <param name="creator"></param>
        void CreateMenu(IMenuCreator creator);
    }
}
