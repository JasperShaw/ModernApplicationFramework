using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : IToolbarCreator
    {
        public ToolBar CreateToolbar(IToolBarHostViewModel model, ToolbarDefinition definition)
        {
            var toolBar = new ToolBar(definition);

            var groups = model.ToolbarItemGroupDefinitions
                .Where(x => x.ParentToolbar == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = model.ToolbarItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var toolBarItem in toolBarItems)
                {
                    var button = new CommandDefinitionButton(toolBarItem);
                    toolBar.Items.Add(button);
                }

                if (i < groups.Count - 1 && toolBarItems.Any())
                    toolBar.Items.Add(new CommandDefinitionButton(CommandBarSeparatorDefinition.MenuSeparatorDefinition));
            }
            return toolBar;
        }
    }
}