using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator : ICreatorBase
    {
    }

    public interface IMainMenuCreator : IMenuCreator
    {
        void CreateMenuBar(IMenuHostViewModel model);

        MenuItem CreateMenuItem(CommandBarDefinitionBase contextMenuDefinition);
    }

    public interface ICreatorBase
    {

        void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition) where T : ItemsControl;

        IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }
}