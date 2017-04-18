using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    public abstract class CreatorBase : ICreatorBase
    {
        public abstract void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition)
            where T : ItemsControl;

        public IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase menuDefinition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == menuDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group).OrderBy(x => x.SortOrder);
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                    if (menuItems.Any())
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        list.Add(separatorDefinition);
                    }
                list.AddRange(menuItems);
            }
            return list;
        }
    }
}