using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class CommandBarMenuController<T> : CommandBarMenuController where T : CommandMenuControllerDefinition
    {
        public CommandBarMenuController(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false)
        {
        }

        public CommandBarMenuController(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool isCustom) :
            base(id, null, (T)IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), group, sortOrder, flags, isCustom)
        {
        }
    }

    public class CommandBarMenuController : CommandBarItem
    {
        protected internal override CommandBarDataSource ItemDataSource { get; }

        public CommandBarMenuController(Guid id, CommandMenuControllerDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, flags, false)
        {
            
        }

        public CommandBarMenuController(Guid id, string name, CommandMenuControllerDefinition itemDefinition, CommandBarGroup group, 
            uint sortOrder, CommandBarFlags flags, bool isCustom)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource = new MenuControllerDataSource(id, name, sortOrder, group, itemDefinition, isCustom, flags);
        }
    }
}
