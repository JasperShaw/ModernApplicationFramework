using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface ICreatorBase
    {

        void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition) where T : ItemsControl;

        IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }
}