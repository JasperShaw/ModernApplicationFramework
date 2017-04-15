using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface ICommandBarHost
    {
        ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }
        ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; } 
        void Build();
        void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator);
    }

    public interface IHasMainWindowViewModel
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }
    }

    public interface IMenuHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        ICommand RightClickCommand { get; }

        ObservableCollection<MenuItem> Items { get; }

        ObservableCollection<MenuBarDefinition> MenuBars { get; }

        bool AllowOpenToolBarContextMenu { get; set; }

        IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions();
    }
}