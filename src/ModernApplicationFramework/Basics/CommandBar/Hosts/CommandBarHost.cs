using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    /// <inheritdoc cref="ICommandBarHost" />
    /// <summary>
    /// Basic implementation of <see cref="ICommandBarHost"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Core.ViewModelBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.ICommandBarHost" />
    public abstract class CommandBarHost : ViewModelBase, ICommandBarHost
    {
        public abstract ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }
        
        public ICommandBarDefinitionHost DefinitionHost { get; }

        [ImportingConstructor]
        protected CommandBarHost()
        {
            DefinitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        public virtual void AddItemDefinition(CommandBarItemDataSource dataSource, CommandBarDataSource parent,
            bool addAboveSeparator)
        {
            //Apparently the current parent is empty so we need to add a group first
            if (dataSource.Group == null)
            {
                var group = new CommandBarGroup(parent, uint.MinValue);
                dataSource.Group = group;
                DefinitionHost.ItemGroupDefinitions.AddSorted(group, new SortOrderComparer<CommandBarDataSource>());
            }

            if (!addAboveSeparator)
                AdjustItemsAfterItemInsertedInGroup(dataSource);
            DefinitionHost.ItemDefinitions.AddSorted(dataSource, new SortOrderComparer<CommandBarDataSource>());
            RemoveGapsInGroupSortOrder(dataSource.Group.Parent);
            BuildLogical(dataSource);
        }

        public virtual void DeleteItemDefinition(CommandBarItemDataSource dataSource)
        {
            //As a Separator contains the previous group we need add all items into the next group
            if (dataSource.UiType == CommandControlTypes.Separator)
            {
                if (dataSource.Group == null || !DefinitionHost.ItemGroupDefinitions.Contains(dataSource.Group))
                    return;
                DeleteGroup(dataSource.Group);
            }
            else
            {
                var definitionsInGroup = dataSource.Group.Items;

                if (definitionsInGroup.Count <= 1)
                {
                    DefinitionHost.ItemGroupDefinitions.Remove(dataSource.Group);
                    RemoveGapsInGroupSortOrder(dataSource.Group.Parent);
                }
                else
                {
                    var definitionsToChange = definitionsInGroup.Where(x => x.SortOrder >= dataSource.SortOrder)
                        .OrderBy(x => x.SortOrder);
                    foreach (var definitionToChange in definitionsToChange)
                    {
                        if (definitionToChange == dataSource)
                            continue;
                        definitionToChange.SortOrder--;
                    }
                }
                DefinitionHost.ItemDefinitions.Remove(dataSource);
                BuildLogical(dataSource.Group.Parent);
                Build(dataSource.Group.Parent);
            }
        }

        public abstract void Build(CommandBarDataSource definition);

        public abstract void Build();

        public void BuildLogical(CommandBarDataSource definition)
        {           
            var groups = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            if (definition is MenuDataSource && groups.Count == 0)
                    definition.IsEnabled = false;
            else
                definition.IsEnabled = true;

            var veryFirstItem = true;
            uint newGroupSortOrder = 0;
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                group.SortOrder = newGroupSortOrder++;
                var menuItems = group.Items
                    .Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);


                var precededBySeparator = false;
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                    //if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        precededBySeparator = true;
                uint newSortOrder = 0;

                foreach (var menuItemDefinition in menuItems)
                {
                    menuItemDefinition.PrecededBySeparator = precededBySeparator;
                    precededBySeparator = false;
                    BuildLogical(menuItemDefinition);
                    menuItemDefinition.SortOrder = newSortOrder++;
                    menuItemDefinition.IsVeryFirst = veryFirstItem;
                    veryFirstItem = false;
                }
            }
        }


        public void BuildLogical(CommandBarDataSource definition, IReadOnlyList<CommandBarGroup> groups, Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> itemFunc)
        {
            var topGroups = groups.Where(x => x.Parent == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            if (definition is MenuDataSource && !groups.Any())
                definition.IsEnabled = false;
            else
                definition.IsEnabled = true;
       
            var veryFirstItem = true;
            uint newGroupSortOrder = 0;

            for (var i = 0; i < topGroups.Count; i++)
            {
                var group = groups[i];
                group.SortOrder = newGroupSortOrder++;

                var items = itemFunc(group);

                var precededBySeparator = false;
                if (i > 0 && i <= groups.Count - 1 && items.Any())
                    if (items.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        precededBySeparator = true;
                uint newSortOrder = 0;

                foreach (var menuItemDefinition in items)
                {
                    menuItemDefinition.PrecededBySeparator = precededBySeparator;
                    precededBySeparator = false;
                    BuildLogical(menuItemDefinition, groups, itemFunc);
                    menuItemDefinition.SortOrder = newSortOrder++;
                    menuItemDefinition.IsVeryFirst = veryFirstItem;
                    veryFirstItem = false;
                }
            }
        }


        public void DeleteGroup(CommandBarGroup group, AppendTo option = AppendTo.Next)
        {
            var definitionsInCurrentGroup = group.Items;
            var newGroup = option == AppendTo.Next ? GetNextGroup(group) : GetPreviousGroup(group);
            var definitionsInNewGroup = newGroup.Items;
            var parent = group.Parent;

            uint newSortorder = 0;
            if (option == AppendTo.Next)
            {
                newSortorder = group.LastItem?.SortOrder ?? 0;


                //Add old items after the new inserted ones
                foreach (var itemDefinition in definitionsInNewGroup.ToList())
                    itemDefinition.SortOrder = newSortorder++;

                foreach (var itemDefinition in definitionsInCurrentGroup.ToList())
                    itemDefinition.Group = newGroup;
            }
            else
            {
                foreach (var itemDefinition in definitionsInNewGroup.ToList())
                    itemDefinition.SortOrder = newSortorder++;

                foreach (var itemDefinition in definitionsInCurrentGroup.ToList())
                {
                    itemDefinition.Group = newGroup;
                    itemDefinition.SortOrder = newSortorder++;
                }
            }
            DefinitionHost.ItemGroupDefinitions.Remove(group);
            RemoveGapsInGroupSortOrder(parent);
            BuildLogical(parent);
        }

        public IEnumerable<CommandBarDataSource> GetMenuHeaderItemDefinitions()
        {
            var list = new List<CommandBarDataSource>();
            IEnumerable<CommandBarDataSource> topDefinitions =
                TopLevelDefinitions/*.OrderBy(x => x.SortOrder)*/.ToList();

            foreach (var barDefinition in topDefinitions)
            {
                list.Add(barDefinition);
                list.AddRange(GetSubHeaderMenus(barDefinition));
            }
            return list;
        }

        public void AddGroupAt(CommandBarItemDataSource startingItem)
        {
            var parent = startingItem.Group.Parent;

            var itemsToRegroup = startingItem.Group.Items
                .Where(x => x.SortOrder >= startingItem.SortOrder);

            var itemsToRegroupInOldGroup = startingItem.Group.Items
                .Where(x => x.SortOrder < startingItem.SortOrder);

            var newGroupSortOrder = startingItem.Group.SortOrder + 1;

            var groupsToResort = parent.ContainedGroups
                .Where(x => x.SortOrder > startingItem.Group.SortOrder);

            var i = newGroupSortOrder + 1;
            foreach (var groupDefinition in groupsToResort)
                groupDefinition.SortOrder = i++;

            var newGroup = new CommandBarGroup(parent, newGroupSortOrder);

            uint j = 0;
            foreach (var itemDefinition in itemsToRegroup.ToList())
            {
                itemDefinition.Group = newGroup;
                itemDefinition.SortOrder = j++;
            }

            uint k = 0;
            foreach (var itemDefinition in itemsToRegroupInOldGroup)
                itemDefinition.SortOrder = k++;

            DefinitionHost.ItemGroupDefinitions.AddSorted(newGroup, new SortOrderComparer<CommandBarGroup>());
            RemoveGapsInGroupSortOrder(parent);
            BuildLogical(parent);
        }

        public void MoveItem(CommandBarItemDataSource item, int offset, CommandBarDataSource parent)
        {
            var sepcounter = Math.Abs(offset);
            if (offset == 0)
                return;

            for (var i = 0; i < sepcounter; i++)
                if (offset < 0)
                    StepwiseMoveUp(item, parent);
                else
                    StepwiseMoveDown(item, parent);
            BuildLogical(parent);
        }

        public CommandBarItemDataSource GetPreviousItem(CommandBarItemDataSource dataSource)
        {
            CommandBarItemDataSource previousItem;
            if (dataSource.UiType == CommandControlTypes.Separator || dataSource.SortOrder == 0)
            {
                if (dataSource.SortOrder != 0)
                    return null;
                var previousGroup = GetPreviousGroup(dataSource.Group);
                if (previousGroup == null)
                    return null;

                previousItem = previousGroup.LastItem;
            }
            else
            {
                previousItem = GetPreviousItemInGroup(dataSource);
            }
            return previousItem;
        }

        public CommandBarItemDataSource GetPreviousItemInGroup(CommandBarItemDataSource dataSource)
        {
            if (dataSource.UiType == CommandControlTypes.Separator)
                return null;
            return DefinitionHost.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == dataSource.Group)
                .OrderByDescending(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder < dataSource.SortOrder);
        }

        public CommandBarItemDataSource GetNextItemInGroup(CommandBarItemDataSource dataSource)
        {
            if (dataSource.UiType == CommandControlTypes.Separator)
                return null;
            return DefinitionHost.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == dataSource.Group)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder > dataSource.SortOrder);
        }

        public CommandBarItemDataSource GetNextItem(CommandBarItemDataSource dataSource)
        {
            CommandBarItemDataSource nextItem;

            var hightestSortOrder = dataSource.Group.LastItem?.SortOrder;

            if (dataSource.UiType == CommandControlTypes.Separator ||
                dataSource.SortOrder == hightestSortOrder)
            {
                var nextGroup = GetNextGroup(dataSource.Group);
                if (nextGroup == null)
                    return null;

                nextItem = nextGroup.FirstItem;
            }
            else
            {
                nextItem = GetNextItemInGroup(dataSource);
            }
            return nextItem;
        }

        public CommandBarGroup GetNextGroup(CommandBarGroup group)
        {
            return group.Parent.ContainedGroups.OrderBy(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder > group.SortOrder);
        }

        public CommandBarGroup GetPreviousGroup(CommandBarGroup group)
        {
            return group.Parent.ContainedGroups.OrderBy(x => x.SortOrder)
                .LastOrDefault(x => x.SortOrder < group.SortOrder);
        }

        private static readonly ICommandBarLayoutBackupProvider LayoutBackupProvider =
            IoC.Get<ICommandBarLayoutBackupProvider>();

        public void Reset(CommandBarDataSource definition)
        {
            IoC.Get<ICommandBarSerializer>().ResetFromBackup(LayoutBackupProvider.Backup, definition);
            Build();
        }

        protected void StepwiseMoveUp(CommandBarItemDataSource item, CommandBarDataSource parent)
        {
            if (item.IsVeryFirst)
                return;
            if (item.UiType == CommandControlTypes.Separator)
            {
                var lastItem = item.Group.LastItem;
                StepwiseMoveDown(lastItem, parent);
            }
            else
            {
                if (item.SortOrder == 0)
                {
                    var previousGroup = GetPreviousGroup(item.Group);
                    var lastGroup = item.Group;
                    item.Group = previousGroup;
                    item.SortOrder = previousGroup.LastItem.SortOrder + 1;

                    var itemsToChange = lastGroup.Items;
                    if (!itemsToChange.Any())
                        DeleteGroup(lastGroup);
                    else
                        foreach (var itemDefinition in itemsToChange.ToList())
                            itemDefinition.SortOrder--;
                }
                else
                {
                    var previousItem = GetPreviousItemInGroup(item);
                    if (previousItem == null)
                        return;
                    previousItem.SortOrder = item.SortOrder;
                    item.SortOrder = item.SortOrder - 1;
                }
            }
        }

        protected void StepwiseMoveDown(CommandBarItemDataSource item, CommandBarDataSource parent)
        {
            var veryLastGroup = GetLastGroupDefinitionInParent(parent);
            var veryLastItem = veryLastGroup.LastItem;
            if (veryLastItem == item)
                return;

            if (item.UiType == CommandControlTypes.Separator)
            {
                var nextGroup = GetNextGroup(item.Group);
                var nextItem = nextGroup.FirstItem;

                StepwiseMoveUp(nextItem, parent);
            }
            else
            {
                var lastItemIndex = item.Group.LastItem?.SortOrder;

                if (lastItemIndex == item.SortOrder)
                {
                    //Add to a new Group
                    var nextGroup = GetNextGroup(item.Group);
                    var lastGroup = item.Group;
                    item.Group = nextGroup;
                    item.SortOrder = 0;
                    AdjustItemsAfterItemInsertedInGroup(item);
                    if (DefinitionHost.ItemDefinitions.OfType<CommandBarItemDataSource>().All(x => x.Group != lastGroup))
                        DeleteGroup(lastGroup);
                }
                else
                {
                    var nextItem = GetNextItemInGroup(item);
                    if (nextItem == null)
                        return;
                    nextItem.SortOrder = item.SortOrder;
                    item.SortOrder = item.SortOrder + 1;
                }
            }
        }

        protected IEnumerable<CommandBarDataSource> GetSubHeaderMenus(CommandBarDataSource definition)
        {
            var list = new List<CommandBarDataSource>();

            var groups = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == definition);

            foreach (var groupDefinition in groups)
            {
                var headerMenus = DefinitionHost.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x is MenuDataSource)
                    .Where(x => x.Group == groupDefinition)
                    .OrderBy(x => x.SortOrder);

                foreach (var headerMenu in headerMenus)
                {
                    list.Add(headerMenu);
                    list.AddRange(GetSubHeaderMenus(headerMenu));
                }
            }
            return list;
        }

        protected void RemoveGapsInGroupSortOrder(CommandBarDataSource parent)
        {
            var groupsToChange = GetAllGroupsInParent(parent);

            uint newSortOrder = 0;
            foreach (var groupDefinition in groupsToChange)
                groupDefinition.SortOrder = newSortOrder++;
        }

        public static CommandBarDataSource FindRoot(CommandBarDataSource definition)
        {
            if (!(definition is CommandBarItemDataSource) && !(definition is CommandBarGroup))
                return definition;
            if (definition is CommandBarGroup groupDefinition)
                return FindRoot(groupDefinition.Parent);
            var itemDefinition = (CommandBarItemDataSource) definition;
            if (itemDefinition.Group == null)
                return itemDefinition;
            return FindRoot(itemDefinition.Group.Parent);
        }

        private static void AdjustItemsAfterItemInsertedInGroup(CommandBarItemDataSource item)
        {
            var definitionsToChange = item.Group.Items
                .Where(x => x.SortOrder >= item.SortOrder)
                .OrderBy(x => x.SortOrder);

            foreach (var definitionToChange in definitionsToChange)
            {
                if (definitionToChange == item)
                    continue;
                definitionToChange.SortOrder++;
            }
        }

        private IEnumerable<CommandBarGroup> GetAllGroupsInParent(CommandBarDataSource parent)
        {
            return parent.ContainedGroups.OrderBy(x => x.SortOrder);
        }

        private CommandBarGroup GetLastGroupDefinitionInParent(CommandBarDataSource parent)
        {
            return GetAllGroupsInParent(parent).LastOrDefault();
        }
    }

    public enum AppendTo
    {
        Next,
        Previous
    }
}