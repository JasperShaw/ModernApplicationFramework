using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
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
                .Where(x => x.Parent == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            uint newSortOrder = 0;
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = model.ToolbarItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                var firstItem = false;    
                if (i > 0 && i <= groups.Count - 1 && toolBarItems.Any())
                {
                    if (toolBarItems.Any(toolbarItemDefinition => toolbarItemDefinition.IsVisible))
                    {
                        var separator = new CommandDefinitionButton(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
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

        public IEnumerable<CommandBarDefinitionBase> GetToolBarItemDefinitions(CommandBarDefinitionBase toolbarDefinition)
        {
            var list = new List<CommandBarDefinitionBase>();

            var model = IoC.Get<IToolBarHostViewModel>();
            var groups = model.ToolbarItemGroupDefinitions
                .Where(x => x.Parent == toolbarDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = model.ToolbarItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                if (i > 0 && i <= groups.Count - 1 && toolBarItems.Any())
                    if (toolBarItems.Any(toolbarItemDefinition => toolbarItemDefinition.IsVisible))
                        list.Add(CommandBarSeparatorDefinition.MenuSeparatorDefinition);

                list.AddRange(toolBarItems);
            }


            return list;
        }
    }
}