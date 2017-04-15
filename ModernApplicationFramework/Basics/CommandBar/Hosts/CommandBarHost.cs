using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    public abstract class CommandBarHost : ViewModelBase, ICommandBarHost
    {
        public ICommandBarDefinitionHost DefinitionHost { get; }

        [ImportingConstructor]
        protected CommandBarHost()
        {
            DefinitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        public abstract void Build();

        public virtual void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator)
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

                var definitionsInCurrnetGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group).OrderBy(x => x.SortOrder).ToList();
                var nextGroup = DefinitionHost.ItemGroupDefinitions.Where(x => x.Parent == parent).FirstOrDefault(x => x.SortOrder > definition.Group.SortOrder);
                var definitionsInNextGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == nextGroup).OrderBy(x => x.SortOrder).ToList();

                uint newSortorder = 0;
                foreach (var groupDefinition in definitionsInCurrnetGroup)
                {
                    groupDefinition.Group = nextGroup;
                    groupDefinition.SortOrder = newSortorder++;
                }

                //Add old items after the new inserted ones
                foreach (var groupDefinition in definitionsInNextGroup)
                    groupDefinition.SortOrder = newSortorder++;

                DefinitionHost.ItemGroupDefinitions.Remove(definition.Group);
            }
            else
            {
                var definitionsInGroup = DefinitionHost.ItemDefinitions.Where(x => x.Group == definition.Group).ToList();

                if (definitionsInGroup.Count <= 1)
                {
                    var groupsToChange = DefinitionHost.ItemGroupDefinitions.Where(x => x.SortOrder >= definition.Group.SortOrder).OrderBy(x => x.SortOrder);

                    foreach (var groupDefinition in groupsToChange)
                    {
                        if (groupDefinition == definition.Group)
                            continue;
                        groupDefinition.SortOrder--;
                    }
                    DefinitionHost.ItemGroupDefinitions.Remove(definition.Group);
                }
                else
                {
                    var definitionsToChange = definitionsInGroup.Where(x => x.SortOrder >= definition.SortOrder).OrderBy(x => x.SortOrder);
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
    }
}
