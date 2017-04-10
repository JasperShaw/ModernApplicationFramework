using System.Collections;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IToolbarCreator
    {
        /// <summary>
        ///     Populate a toolbartray with a ToolbarDefinitionsPopulator
        /// </summary>
        /// <param name="model"></param>
        ToolBar CreateToolbar(IToolBarHostViewModel model, ToolbarDefinition definition);

        IEnumerable GetToolBarItemDefinitions(CommandBarDefinitionBase selectedToolBarItem);
    }
}