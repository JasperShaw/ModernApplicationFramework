using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IToolbarCreator
    {
        /// <summary>
        ///     Populate a toolbartray with a ToolbarDefinitionsPopulator
        /// </summary>
        ToolBar CreateToolbar(ToolbarDefinition definition);

        IEnumerable<CommandBarItemDefinition> GetToolBarItemDefinitions(CommandBarDefinitionBase selectedToolBarItem);
    }
}