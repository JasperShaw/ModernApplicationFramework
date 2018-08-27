using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public class CommandBarCommandItem<T> : CommandBarCommandItem where T : CommandItemDefinitionBase
    {
        public CommandBarCommandItem(Guid id, CommandBarGroup group, uint sortOrder, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false, false)
        {
            
        }

        public CommandBarCommandItem(Guid id, CommandBarGroup group, uint sortOrder, CommandBarFlags flags, bool isCustom, bool isChecked) : 
            base(id, null, (T)IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), group, sortOrder, isCustom, isChecked, flags)
        {
            
        }
    }

    public class CommandBarCommandItem : CommandBarItem
    {
        protected internal override CommandBarDataSource ItemDataSource { get; }

        public CommandBarCommandItem(Guid id, string name, CommandItemDefinitionBase itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, name, itemDefinition, group, sortOrder, false, false, flags)
        {
            
        }

        public CommandBarCommandItem(Guid id, string name, CommandItemDefinitionBase itemDefinition, CommandBarGroup group, uint sortOrder, 
            bool isCustom, bool isChecked, CommandBarFlags flags)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource =
                new ButtonDataSource(id, name, sortOrder, group, itemDefinition, isCustom, isChecked, flags);
        }
    }
}
