using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.CommandBar.Elements
{
    public sealed class CommandBarComboBox<T> : CommandBarComboBox where T : ComboBoxDefinition
    {
        public CommandBarComboBox(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, false)
        {
        }

        public CommandBarComboBox(Guid id, CommandBarGroup group, uint sortOrder, 
            CommandBarFlags flags, bool isCustom) : 
            base(id, null, (T)IoC.Get<ICommandBarItemService>().GetItemDefinition(typeof(T)), group, sortOrder, flags, isCustom)
        {
        }
    }

    public class CommandBarComboBox : CommandBarItem
    {
        public override CommandBarDataSource ItemDataSource { get; }

        public CommandBarComboBox(Guid id, ComboBoxDefinition itemDefinition, CommandBarGroup group, uint sortOrder, 
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, flags, false)
        {

        }

        public CommandBarComboBox(Guid id, string name, ComboBoxDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool isCustom)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource = new ComboBoxDataSource(id, name, sortOrder, group,
                itemDefinition, isCustom, flags);
        }   
    }
}

