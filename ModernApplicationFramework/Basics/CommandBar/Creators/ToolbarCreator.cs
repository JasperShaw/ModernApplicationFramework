using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : MenuCreatorBase, IToolbarCreator
    {
        public ToolBar CreateToolbar(ToolbarDefinition definition)
        {
            var toolBar = new ToolBar(definition);

            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = host.ItemGroupDefinitions
                .Where(x => x.Parent == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            var veryFirstItem = true;
            for (var i = 0; i < groups.Count; i++)
            {
                uint newSortOrder = 0;
                var group = groups[i];
                var toolBarItems = host.ItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                var precededBySeparator = false;
                if (i > 0 && i <= groups.Count - 1 && toolBarItems.Any())
                {
                    if (toolBarItems.Any(toolbarItemDefinition => toolbarItemDefinition.IsVisible))
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        var separator = new CommandDefinitionButton(separatorDefinition);
                        toolBar.Items.Add(separator);
                        precededBySeparator = true;
                    }
                }

                foreach (var toolBarItem in toolBarItems)
                {
                    toolBarItem.PrecededBySeparator = precededBySeparator;
                    var button = new CommandDefinitionButton(toolBarItem);
                    toolBar.Items.Add(button);
                    precededBySeparator = false;
                    toolBarItem.SortOrder = newSortOrder++;
                    toolBarItem.IsVeryFirst = veryFirstItem;
                    veryFirstItem = false;
                }
            }
            return toolBar;
        }
    }
}