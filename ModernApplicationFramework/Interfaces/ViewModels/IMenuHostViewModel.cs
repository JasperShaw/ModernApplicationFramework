using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface ICommandBarHost
    {
        ICommandBarDefinitionHost DefinitionHost { get; }

        void Build();
        void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator);

        void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent); //, bool addAboveSeparator);
        CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition, CommandBarDefinitionBase parent);

        CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition, CommandBarDefinitionBase parent);

        CommandBarItemDefinition GetNextItemInGroup(CommandBarItemDefinition definition);

        CommandBarItemDefinition GetPreviousItemInGroup(CommandBarItemDefinition definition);
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