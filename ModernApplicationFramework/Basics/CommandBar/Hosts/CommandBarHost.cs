using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
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
        public abstract ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }
        
        public ICommandBarDefinitionHost DefinitionHost { get; }

        [ImportingConstructor]
        protected CommandBarHost()
        {
            DefinitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        public virtual void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent,
            bool addAboveSeparator)
        {
            //Apparently the current parent is empty so we need to add a group first
            if (definition.Group == null)
            {
                var group = new CommandBarGroupDefinition(parent, uint.MinValue);
                definition.Group = group;
                DefinitionHost.ItemGroupDefinitions.AddSorted(group, new SortOrderComparer<CommandBarDefinitionBase>());
            }

            if (!addAboveSeparator)
                AdjustItemsAfterItemInsertedInGroup(definition);
            DefinitionHost.ItemDefinitions.AddSorted(definition, new SortOrderComparer<CommandBarDefinitionBase>());
            RemoveGapsInGroupSortOrder(definition.Group.Parent);
            BuildLogical(definition);
        }

        public virtual void DeleteItemDefinition(CommandBarItemDefinition definition)
        {
            //As a Separator contains the previous group we need add all items into the next group
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {
                if (definition.Group == null || !DefinitionHost.ItemGroupDefinitions.Contains(definition.Group))
                    return;
                DeleteGroup(definition.Group);
            }
            else
            {
                var definitionsInGroup = definition.Group.Items;

                if (definitionsInGroup.Count <= 1)
                {
                    DefinitionHost.ItemGroupDefinitions.Remove(definition.Group);
                    RemoveGapsInGroupSortOrder(definition.Group.Parent);
                }
                else
                {
                    var definitionsToChange = definitionsInGroup.Where(x => x.SortOrder >= definition.SortOrder)
                        .OrderBy(x => x.SortOrder);
                    foreach (var definitionToChange in definitionsToChange)
                    {
                        if (definitionToChange == definition)
                            continue;
                        definitionToChange.SortOrder--;
                    }
                }
                DefinitionHost.ItemDefinitions.Remove(definition);
                BuildLogical(definition.Group.Parent);
                Build(definition.Group.Parent);
            }
        }

        public abstract void Build(CommandBarDefinitionBase definition);

        public abstract void Build();

        public void BuildLogical(CommandBarDefinitionBase definition)
        {           
            var groups = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            if (definition is MenuDefinition && groups.Count == 0)
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
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
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

        public void DeleteGroup(CommandBarGroupDefinition group, AppendTo option = AppendTo.Next)
        {
            var definitionsInCurrnetGroup = group.Items;
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

                foreach (var itemDefinition in definitionsInCurrnetGroup.ToList())
                    itemDefinition.Group = newGroup;
            }
            else
            {
                foreach (var itemDefinition in definitionsInNewGroup.ToList())
                    itemDefinition.SortOrder = newSortorder++;

                foreach (var itemDefinition in definitionsInCurrnetGroup.ToList())
                {
                    itemDefinition.Group = newGroup;
                    itemDefinition.SortOrder = newSortorder++;
                }
            }
            DefinitionHost.ItemGroupDefinitions.Remove(group);
            RemoveGapsInGroupSortOrder(parent);
            BuildLogical(parent);
        }

        public IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions()
        {
            var list = new List<CommandBarDefinitionBase>();
            IEnumerable<CommandBarDefinitionBase> topDefinitions =
                TopLevelDefinitions.OrderBy(x => x.SortOrder).ToList();

            foreach (var barDefinition in topDefinitions)
            {
                list.Add(barDefinition);
                list.AddRange(GetSubHeaderMenus(barDefinition));
            }
            return list;
        }

        public void AddGroupAt(CommandBarItemDefinition startingDefinition)
        {
            var parent = startingDefinition.Group.Parent;

            var itemsToRegroup = startingDefinition.Group.Items
                .Where(x => x.SortOrder >= startingDefinition.SortOrder);

            var itemsToRegroupInOldGroup = startingDefinition.Group.Items
                .Where(x => x.SortOrder < startingDefinition.SortOrder);

            var newGroupSortOrder = startingDefinition.Group.SortOrder + 1;

            var groupsToResort = parent.ContainedGroups
                .Where(x => x.SortOrder > startingDefinition.Group.SortOrder);

            var i = newGroupSortOrder + 1;
            foreach (var groupDefinition in groupsToResort)
                groupDefinition.SortOrder = i++;

            var newGroup = new CommandBarGroupDefinition(parent, newGroupSortOrder);

            uint j = 0;
            foreach (var itemDefinition in itemsToRegroup.ToList())
            {
                itemDefinition.Group = newGroup;
                itemDefinition.SortOrder = j++;
            }

            uint k = 0;
            foreach (var itemDefinition in itemsToRegroupInOldGroup)
                itemDefinition.SortOrder = k++;

            DefinitionHost.ItemGroupDefinitions.AddSorted(newGroup, new SortOrderComparer<CommandBarGroupDefinition>());
            RemoveGapsInGroupSortOrder(parent);
            BuildLogical(parent);
        }

        public void MoveItem(CommandBarItemDefinition item, int offset, CommandBarDefinitionBase parent)
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

        public CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition)
        {
            CommandBarItemDefinition previousItem;
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator || definition.SortOrder == 0)
            {
                if (definition.SortOrder != 0)
                    return null;
                var previousGroup = GetPreviousGroup(definition.Group);
                if (previousGroup == null)
                    return null;

                previousItem = previousGroup.LastItem;
            }
            else
            {
                previousItem = GetPreviousItemInGroup(definition);
            }
            return previousItem;
        }

        public CommandBarItemDefinition GetPreviousItemInGroup(CommandBarItemDefinition definition)
        {
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
                return null;
            return DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group)
                .OrderByDescending(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder < definition.SortOrder);
        }

        public CommandBarItemDefinition GetNextItemInGroup(CommandBarItemDefinition definition)
        {
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
                return null;
            return DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder > definition.SortOrder);
        }

        public CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition)
        {
            CommandBarItemDefinition nextItem;

            var hightestSortOrder = definition.Group.LastItem?.SortOrder;

            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator ||
                definition.SortOrder == hightestSortOrder)
            {
                var nextGroup = GetNextGroup(definition.Group);
                if (nextGroup == null)
                    return null;

                nextItem = nextGroup.FirstItem;
            }
            else
            {
                nextItem = GetNextItemInGroup(definition);
            }
            return nextItem;
        }

        public CommandBarGroupDefinition GetNextGroup(CommandBarGroupDefinition group)
        {
            return group.Parent.ContainedGroups.OrderBy(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder > group.SortOrder);
        }

        public CommandBarGroupDefinition GetPreviousGroup(CommandBarGroupDefinition group)
        {
            return group.Parent.ContainedGroups.OrderBy(x => x.SortOrder)
                .LastOrDefault(x => x.SortOrder < group.SortOrder);
        }

        protected void StepwiseMoveUp(CommandBarItemDefinition item, CommandBarDefinitionBase parent)
        {
            if (item.IsVeryFirst)
                return;
            if (item.CommandDefinition.ControlType == CommandControlTypes.Separator)
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

        protected void StepwiseMoveDown(CommandBarItemDefinition item, CommandBarDefinitionBase parent)
        {
            var veryLastGroup = GetLastGroupDefinitionInParent(parent);
            var veryLastItem = veryLastGroup.LastItem;
            if (veryLastItem == item)
                return;

            if (item.CommandDefinition.ControlType == CommandControlTypes.Separator)
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
                    if (DefinitionHost.ItemDefinitions.All(x => x.Group != lastGroup))
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

        protected IEnumerable<CommandBarDefinitionBase> GetSubHeaderMenus(CommandBarDefinitionBase definition)
        {
            var list = new List<CommandBarDefinitionBase>();

            var groups = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == definition);

            foreach (var groupDefinition in groups)
            {
                var headerMenus = DefinitionHost.ItemDefinitions.Where(x => x is MenuDefinition)
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

        protected void RemoveGapsInGroupSortOrder(CommandBarDefinitionBase parent)
        {
            var groupsToChange = GetAllGroupsInParent(parent);

            uint newSortOrder = 0;
            foreach (var groupDefinition in groupsToChange)
                groupDefinition.SortOrder = newSortOrder++;
        }

        private static void AdjustItemsAfterItemInsertedInGroup(CommandBarItemDefinition item)
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

        private IEnumerable<CommandBarGroupDefinition> GetAllGroupsInParent(CommandBarDefinitionBase parent)
        {
            return parent.ContainedGroups.OrderBy(x => x.SortOrder);
        }

        private CommandBarGroupDefinition GetLastGroupDefinitionInParent(CommandBarDefinitionBase parent)
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