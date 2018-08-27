using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class CommandBarSplitButton<T> : CommandBarSplitButton where T : CommandSplitButtonDefinition
    {
        public CommandBarSplitButton(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false)
        {
        }

        public CommandBarSplitButton(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool isCustom) :
            base(id, null, (T)IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), group, sortOrder, isCustom, flags)
        {
        }
    }

    public class CommandBarSplitButton : CommandBarItem
    {
        protected internal override CommandBarItemDataSource ItemDataSource { get; }

        public CommandBarSplitButton(Guid id, CommandSplitButtonDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, false, flags)
        {

        }

        public CommandBarSplitButton(Guid id, string name, CommandSplitButtonDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            bool isCustom, CommandBarFlags flags)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource = new SplitButtonDataSource(id, name, sortOrder, group, itemDefinition, isCustom, flags);
        }
    }
}