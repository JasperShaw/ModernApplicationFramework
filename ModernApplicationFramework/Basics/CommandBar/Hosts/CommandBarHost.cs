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
            RemoveGapsInGroupSortOrder(parent);
        }

        public virtual void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent)
        {
            //As a Separator contains the previous group we need add all items into the next group
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {
                if (definition.Group == null || !DefinitionHost.ItemGroupDefinitions.Contains(definition.Group))
                    return;
                DeleteGroup(definition.Group, parent);
            }
            else
            {
                var definitionsInGroup = GetAllItemsInGroup(definition.Group).ToList();

                if (definitionsInGroup.Count <= 1)
                {
                    DefinitionHost.ItemGroupDefinitions.Remove(definition.Group);
                    RemoveGapsInGroupSortOrder(parent);
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
            }
        }

        public abstract void Build();

        public void DeleteGroup(CommandBarGroupDefinition group, CommandBarDefinitionBase parent,
            AppendTo option = AppendTo.Next)
        {
            var definitionsInCurrnetGroup = GetAllItemsInGroup(group).ToList();
            var newGroup = option == AppendTo.Next ? GetNextGroup(group, parent) : GetPreviousGroup(group, parent);
            var definitionsInNewGroup = GetAllItemsInGroup(newGroup).ToList();

            uint newSortorder = 0;
            if (option == AppendTo.Next)
            {
                foreach (var itemDefinition in definitionsInCurrnetGroup)
                {
                    itemDefinition.Group = newGroup;
                    itemDefinition.SortOrder = newSortorder++;
                }

                //Add old items after the new inserted ones
                foreach (var itemDefinition in definitionsInNewGroup)
                    itemDefinition.SortOrder = newSortorder++;
            }
            else
            {
                foreach (var itemDefinition in definitionsInNewGroup)
                    itemDefinition.SortOrder = newSortorder++;

                foreach (var itemDefinition in definitionsInCurrnetGroup)
                {
                    itemDefinition.Group = newGroup;
                    itemDefinition.SortOrder = newSortorder++;
                }
            }     
            DefinitionHost.ItemGroupDefinitions.Remove(group);
            RemoveGapsInGroupSortOrder(parent);
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

        public void AddGroupAt(CommandBarItemDefinition startingDefinition, CommandBarDefinitionBase parent)
        {
            var itemsToRegroup = GetAllItemsInGroup(startingDefinition.Group)
                .Where(x => x.SortOrder >= startingDefinition.SortOrder);

            var newGroupSortOrder = startingDefinition.Group.SortOrder + 1;

            var groupsToResort = GetAllGroupsInParent(parent)
                .Where(x => x.SortOrder > startingDefinition.Group.SortOrder);

            var i = newGroupSortOrder + 1;
            foreach (var groupDefinition in groupsToResort)
                groupDefinition.SortOrder = i++;

            var newGroup = new CommandBarGroupDefinition(parent, newGroupSortOrder);
            
            uint j = 0;
            foreach (var itemDefinition in itemsToRegroup)
            {
                itemDefinition.Group = newGroup;
                itemDefinition.SortOrder = j++;
            }
            
            DefinitionHost.ItemGroupDefinitions.Add(newGroup);
            RemoveGapsInGroupSortOrder(parent);
        }

        public void MoveItem(CommandBarItemDefinition item, int offset, CommandBarDefinitionBase parent)
        {
            var sepcounter = Math.Abs(offset);
            if (offset == 0)
                return;

            for (var i = 0; i < sepcounter; i++)
            {
                if(offset < 0)
                    StepwiseMoveUp(item, parent);
                else
                    StepwiseMoveDown(item, parent);
            }
        }

        protected void StepwiseMoveUp(CommandBarItemDefinition item, CommandBarDefinitionBase parent)
        {
            if (item.IsVeryFirst)
                return;
            if (item.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {
                var lastItem = GetLastDefinitionInGroup(item.Group);
                StepwiseMoveDown(lastItem as CommandBarItemDefinition, parent);
            }
            else
            {
                if (item.SortOrder == 0)
                {
                    var previousGroup = GetPreviousGroup(item.Group, parent);
                    var lastGroup = item.Group;
                    item.Group = previousGroup;
                    item.SortOrder = GetLastDefinitionInGroup(previousGroup).SortOrder +1;

                    var itemsToChange = GetAllItemsInGroup(lastGroup).ToList();
                    if (!itemsToChange.Any())
                        DeleteGroup(lastGroup, parent);
                    else
                        foreach (var itemDefinition in itemsToChange)
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
            var veryLastItem = GetLastDefinitionInGroup(veryLastGroup);
            if (veryLastItem == item)
                return;

            if (item.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {
                var nextGroup = GetNextGroup(item.Group, parent);
                var nextItem = GetFirstDefinitionInGroup(nextGroup);

                StepwiseMoveUp(nextItem as CommandBarItemDefinition, parent);
            }
            else
            {
                var lastItemIndex = GetLastDefinitionInGroup(item.Group).SortOrder;

                if (lastItemIndex == item.SortOrder)
                {
                    //Add to a new Group
                    var nextGroup = GetNextGroup(item.Group, parent);
                    var lastGroup = item.Group;
                    item.Group = nextGroup;
                    item.SortOrder = 0;
                    AdjustItemsAfterItemInsertedInGroup(item);
                    if (DefinitionHost.ItemDefinitions.All(x => x.Group != lastGroup))
                        DeleteGroup(lastGroup, parent);
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
 
        public CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition,
            CommandBarDefinitionBase parent)
        {
            CommandBarItemDefinition previousItem;
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator || definition.SortOrder == 0)
            {
                if (definition.SortOrder != 0)
                    return null;
                var previousGroup = GetPreviousGroup(definition.Group, parent);
                if (previousGroup == null)
                    return null;

                previousItem = DefinitionHost.ItemDefinitions.LastOrDefault(x => x.Group == previousGroup);
            }
            else
            {
                previousItem = GetPreviousItemInGroup(definition);
            }
            return previousItem;
        }

        public IEnumerable<CommandBarItemDefinition> GetAllItemsInGroup(CommandBarGroupDefinition group)
        {
            return DefinitionHost.ItemDefinitions.Where(x => x.Group == group)
                .OrderBy(x => x.SortOrder);
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

        public CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition,
            CommandBarDefinitionBase parent)
        {
            CommandBarItemDefinition nextItem;

            var hightestSortOrder = DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group)
                .OrderBy(x => x.SortOrder)
                .LastOrDefault()
                ?.SortOrder;

            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator ||
                definition.SortOrder == hightestSortOrder)
            {
                var nextGroup = GetNextGroup(definition.Group, parent);
                if (nextGroup == null)
                    return null;

                nextItem = DefinitionHost.ItemDefinitions.Where(x => x.Group == nextGroup).OrderBy(x => x.SortOrder).FirstOrDefault();
            }
            else
            {
                nextItem = GetNextItemInGroup(definition);
            }
            return nextItem;
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
            var groupsToChange = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent)
                .OrderBy(x => x.SortOrder);

            uint newSortOrder = 0;
            foreach (var groupDefinition in groupsToChange)
            {
                groupDefinition.SortOrder = newSortOrder++;
            }
        }

        public CommandBarGroupDefinition GetNextGroup(CommandBarGroupDefinition group, CommandBarDefinitionBase parent)
        {
            return DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent).OrderBy(x => x.SortOrder)
                .FirstOrDefault(x => x.SortOrder > group.SortOrder);
        }

        public CommandBarGroupDefinition GetPreviousGroup(CommandBarGroupDefinition group,
            CommandBarDefinitionBase parent)
        {
            return DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent).OrderBy(x => x.SortOrder)
                .LastOrDefault(x => x.SortOrder < group.SortOrder);
        }

        private void AdjustItemsAfterItemInsertedInGroup(CommandBarItemDefinition item)
        {
            var definitionsToChange = DefinitionHost.ItemDefinitions.Where(x => x.Group == item.Group)
                .Where(x => x.SortOrder >= item.SortOrder)
                .OrderBy(x => x.SortOrder);

            foreach (var definitionToChange in definitionsToChange)
            {
                if (definitionToChange == item)
                    continue;
                definitionToChange.SortOrder++;
            }
        }

        private CommandBarDefinitionBase GetLastDefinitionInGroup(CommandBarGroupDefinition groupDefinition)
        {
            return DefinitionHost.ItemDefinitions.Where(x => x.Group == groupDefinition)
                .OrderBy(x => x.SortOrder)
                .LastOrDefault();
        }

        private CommandBarDefinitionBase GetFirstDefinitionInGroup(CommandBarGroupDefinition groupDefinition)
        {
            return DefinitionHost.ItemDefinitions.Where(x => x.Group == groupDefinition)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault();
        }

        private IEnumerable<CommandBarGroupDefinition> GetAllGroupsInParent(CommandBarDefinitionBase parent)
        {
            return DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent).OrderBy(x => x.SortOrder);
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