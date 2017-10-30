using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the basic structure required for a menu hoster
    /// </summary>
    public interface IMenuHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        /// <summary>
        /// The command to handle a right click onto the menu host
        /// </summary>
        ICommand RightClickCommand { get; }

        /// <summary>
        /// A collection of the containing menu items
        /// </summary>
        ObservableCollection<MenuItem> Items { get; }

        /// <summary>
        /// An option to link the <see cref="RightClickCommand"/> from an <see cref="IToolBarHostViewModel"/> instance
        /// </summary>
        bool AllowOpenToolBarContextMenu { get; set; }
    }
}