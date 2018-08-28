using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.CommandBar.Elements
{
    public sealed class CommandBarMenuController<T> : CommandBarMenuController where T : MenuControllerDefinition
    {
        public CommandBarMenuController(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false)
        {
        }

        public CommandBarMenuController(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool isCustom) :
            base(id, null, (T)IoC.Get<ICommandBarItemService>().GetItemDefinition(typeof(T)), group, sortOrder, flags, isCustom)
        {
        }
    }

    public class CommandBarMenuController : CommandBarItem
    {
        public override CommandBarDataSource ItemDataSource { get; }

        public CommandBarMenuController(Guid id, MenuControllerDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, flags, false)
        {
            
        }

        public CommandBarMenuController(Guid id, string name, MenuControllerDefinition itemDefinition, CommandBarGroup group, 
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
