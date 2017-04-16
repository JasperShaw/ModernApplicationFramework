using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Core;
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
            //Apparently the current contextmenu is empty so we need to add a group first
            if (definition.Group == null)
            {
                var group = new CommandBarGroupDefinition(parent, uint.MinValue);
                definition.Group = group;
                DefinitionHost.ItemGroupDefinitions.Add(group);
            }

            if (!addAboveSeparator)
            {
                var definitionsToChange = DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group)
                    .Where(x => x.SortOrder >= definition.SortOrder)
                    .OrderBy(x => x.SortOrder);

                foreach (var definitionToChange in definitionsToChange)
                {
                    if (definitionToChange == definition)
                        continue;
                    definitionToChange.SortOrder++;
                }
            }
            DefinitionHost.ItemDefinitions.Add(definition);
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
                var definitionsInGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group)
                    .ToList();

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
            var definitionsInCurrnetGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == group)
                .OrderBy(x => x.SortOrder)
                .ToList();

            var newGroup = option == AppendTo.Next ? NextGroup(group, parent) : PreviousGroup(group, parent);
            var definitionsInNewGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == newGroup)
                .OrderBy(x => x.SortOrder)
                .ToList();

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
            IEnumerable<CommandBarDefinitionBase> barDefinitions =
                TopLevelDefinitions.OrderBy(x => x.SortOrder).ToList();

            foreach (var barDefinition in barDefinitions)
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

            var groupsToResort = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent)
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
            RemoveGapsInGroupSortOrder(parent);
            DefinitionHost.ItemGroupDefinitions.Add(newGroup);
        }

        public CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition,
            CommandBarDefinitionBase parent)
        {
            CommandBarItemDefinition previousItem;
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator || definition.SortOrder == 0)
            {
                if (definition.SortOrder != 0)
                    return null;
                var previousGroup = PreviousGroup(definition.Group, parent);
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
                var nextGroup = NextGroup(definition.Group, parent);
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
            var group = DefinitionHost.ItemGroupDefinitions.FirstOrDefault(x => x.Parent == definition);
            var list = new List<CommandBarDefinitionBase>();
            var headerMenus = DefinitionHost.ItemDefinitions.Where(x => x is MenuDefinition)
                .Where(x => x.Group == group)
                .OrderBy(x => x.SortOrder);

            foreach (var headerMenu in headerMenus)
            {
                list.Add(headerMenu);
                list.AddRange(GetSubHeaderMenus(headerMenu));
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

        private CommandBarGroupDefinition NextGroup(CommandBarGroupDefinition group, CommandBarDefinitionBase parent)
        {
            return DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent)
                .FirstOrDefault(x => x.SortOrder > group.SortOrder);
        }

        private CommandBarGroupDefinition PreviousGroup(CommandBarGroupDefinition group,
            CommandBarDefinitionBase parent)
        {
            return DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent)
                .FirstOrDefault(x => x.SortOrder < group.SortOrder);
        }
    }

    public enum AppendTo
    {
        Next,
        Previous
    }
}