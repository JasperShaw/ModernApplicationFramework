using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class CommandBarSplitButton<T> : CommandBarSplitButton where T : SplitButtonDefinition
    {
        public CommandBarSplitButton(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false, false)
        {
        }

        public CommandBarSplitButton(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool isCustom, bool isChecked) :
            base(id, null, (T)IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), group, sortOrder, isCustom, isChecked, flags)
        {
        }
    }

    public class CommandBarSplitButton : CommandBarItem
    {
        public override CommandBarDataSource ItemDataSource { get; }

        public CommandBarSplitButton(Guid id, SplitButtonDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, false, false, flags)
        {

        }

        public CommandBarSplitButton(Guid id, string name, SplitButtonDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            bool isCustom, bool isChecked, CommandBarFlags flags)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource = new SplitButtonDataSource(id, name, sortOrder, group, itemDefinition, isCustom, isChecked, flags);
        }
    }
}