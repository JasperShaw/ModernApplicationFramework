using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IMainMenuCreator))]
    public class MenuCreator : MenuCreatorBase, IMainMenuCreator
    {
        public void CreateMenuBar(IMenuHostViewModel model)
        {
            model.Items.Clear();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var bars = model.TopLevelDefinitions.OrderBy(x => x.SortOrder);

            foreach (var bar in bars)
            {
                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == bar)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var veryFirstItem = true;
                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var topLevelMenus = host.ItemDefinitions.Where(x => !host.ExcludedItemDefinitions.Contains(x))
                        .Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);


                    var precededBySeparator = false;
                    if (i > 0 && i <= groups.Count - 1 && topLevelMenus.Any())
                    {
                        if (topLevelMenus.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        {
                            var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                            separatorDefinition.Group = groups[i - 1];
                            var separator = new MenuItem(separatorDefinition);
                            model.Items.Add(separator);
                            precededBySeparator = true;
                        }
                    }

                    uint newSortOrder = 0;
                    foreach (var menuDefinition in topLevelMenus)
                    {
                        var menuItem = new MenuItem(menuDefinition);
                        CreateMenuTree(menuDefinition, menuItem);
                        model.Items.Add(menuItem);
                        menuDefinition.SortOrder = newSortOrder++;
                        menuDefinition.IsVeryFirst = veryFirstItem;
                        veryFirstItem = false;
                        menuDefinition.PrecededBySeparator = precededBySeparator;
                        precededBySeparator = false;
                    }
                }
            }
        }
    }
}