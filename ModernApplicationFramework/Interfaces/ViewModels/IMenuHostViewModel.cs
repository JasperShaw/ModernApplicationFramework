using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMenuHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        ICommand RightClickCommand { get; }

        ObservableCollection<MenuItem> Items { get; }

        bool AllowOpenToolBarContextMenu { get; set; }
    }
}