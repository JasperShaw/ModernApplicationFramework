using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMenuHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        ICommand RightClickCommand { get; }

        ObservableCollection<MenuItem> Items { get; }

        ObservableCollection<MenuBarDefinition> MenuBars { get; }

        bool AllowOpenToolBarContextMenu { get; set; }

        IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions();
    }
}