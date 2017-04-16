using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator
    {
        void CreateMenuTree(CommandBarDefinitionBase definition, ItemsControl menuItem);

        IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }

    public interface IMainMenuCreator : IMenuCreator
    {
        void CreateMenuBar(IMenuHostViewModel model);
    }
}