using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the basic structure required for a toolbar hoster
    /// </summary>
    public interface IToolBarHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        /// <summary>
        /// The command to handle a right click onto the toolbar host
        /// </summary>
        ICommand OpenContextMenuCommand { get; }

        /// <summary>
        /// The context menu of the toolbar host
        /// </summary>
        ContextMenu ContextMenu { get; }

        /// <summary>
        /// The top toolbar tray
        /// </summary>
        ToolBarTray TopToolBarTray { get; set; }

        /// <summary>
        /// The left toolbar tray
        /// </summary>
        ToolBarTray LeftToolBarTray { get; set; }

        /// <summary>
        /// The right toolbar tray
        /// </summary>
        ToolBarTray RightToolBarTray { get; set; }

        /// <summary>
        /// The bottom toolbar tray
        /// </summary>
        ToolBarTray BottomToolBarTray { get; set; }


        /// <summary>
        /// Defines the toolbar scope of the host.
        /// </summary>
        ToolbarScope ToolbarScope { get; }


        /// <summary>
        /// Gets an unique localized default toolbar name
        /// </summary>
        /// <returns></returns>
        string GetUniqueToolBarName();
    }
}