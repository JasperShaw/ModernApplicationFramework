using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : IToolbarCreator
    {
        public ToolBar CreateToolbar(ToolbarDefinition definition)
        {
            var toolBar = new ToolBar(definition);

            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = host.ItemGroupDefinitions
                .Where(x => x.Parent == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            
            for (var i = 0; i < groups.Count; i++)
            {
                uint newSortOrder = 0;
                var group = groups[i];
                var toolBarItems = host.ItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                var firstItem = false;    
                if (i > 0 && i <= groups.Count - 1 && toolBarItems.Any())
                {
                    if (toolBarItems.Any(toolbarItemDefinition => toolbarItemDefinition.IsVisible))
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        var separator = new CommandDefinitionButton(separatorDefinition);
                        toolBar.Items.Add(separator);
                        firstItem = true;
                    }
                }

                foreach (var toolBarItem in toolBarItems)
                {
                    toolBarItem.PrecededBySeparator = firstItem;
                    var button = new CommandDefinitionButton(toolBarItem);
                    toolBar.Items.Add(button);
                    firstItem = false;
                    toolBarItem.SortOrder = newSortOrder++;
                }
            }
            return toolBar;
        }

        public IEnumerable<CommandBarItemDefinition> GetToolBarItemDefinitions(CommandBarDefinitionBase toolbarDefinition)
        {
            var list = new List<CommandBarItemDefinition>();

            var model = IoC.Get<ICommandBarDefinitionHost>();
            var groups = model.ItemGroupDefinitions
                .Where(x => x.Parent == toolbarDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = model.ItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                if (i > 0 && i <= groups.Count - 1 && toolBarItems.Any())
                    if (toolBarItems.Any(toolbarItemDefinition => toolbarItemDefinition.IsVisible))
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        list.Add(separatorDefinition);
                    }

                list.AddRange(toolBarItems);
            }
            return list;
        }
    }
}