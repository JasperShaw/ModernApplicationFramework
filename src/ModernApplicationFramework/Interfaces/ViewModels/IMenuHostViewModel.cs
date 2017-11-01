using System.Collections.ObjectModel;
using System.ComponentModel;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the basic structure required for a menu hoster
    /// </summary>
    public interface IMenuHostViewModel : ICommandBarHost, IHasMainWindowViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// A collection of the containing menu items
        /// </summary>
        ObservableCollection<MenuItem> Items { get; }

        /// <summary>
        /// Allow the Main Menu to show the context menu of the ToolBarTray
        /// </summary>
        bool AllowOpenToolBarContextMenu { get; set; }

        System.Windows.Controls.ContextMenu ContextMenu { get; }
    }
}