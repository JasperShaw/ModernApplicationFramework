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
                    RearrangeGroups(definition.Group);
                    DefinitionHost.ItemGroupDefinitions.Remove(definition.Group);
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
                foreach (var groupDefinition in definitionsInCurrnetGroup)
                {
                    groupDefinition.Group = newGroup;
                    groupDefinition.SortOrder = newSortorder++;
                }

                //Add old items after the new inserted ones
                foreach (var groupDefinition in definitionsInNewGroup)
                    groupDefinition.SortOrder = newSortorder++;
            }
            else
            {
                foreach (var groupDefinition in definitionsInNewGroup)
                    groupDefinition.SortOrder = newSortorder++;

                foreach (var groupDefinition in definitionsInCurrnetGroup)
                {
                    groupDefinition.Group = newGroup;
                    groupDefinition.SortOrder = newSortorder++;
                }
            }
            RearrangeGroups(group);
            DefinitionHost.ItemGroupDefinitions.Remove(group);
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

                nextItem = DefinitionHost.ItemDefinitions.FirstOrDefault(x => x.Group == nextGroup);
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


        private void RearrangeGroups(CommandBarGroupDefinition deletedGroup)
        {
            var groupsToChange = DefinitionHost.ItemGroupDefinitions.Where(x => x.SortOrder >= deletedGroup.SortOrder)
                .OrderBy(x => x.SortOrder);

            foreach (var groupDefinition in groupsToChange)
            {
                if (groupDefinition == deletedGroup)
                    continue;
                groupDefinition.SortOrder--;
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