using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class CommandBarComboBox<T> : CommandBarComboBox where T : CommandComboBoxDefinition
    {
        public CommandBarComboBox(Guid id, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            this(id, group, sortOrder, flags, true, false, true)
        {
        }

        public CommandBarComboBox(Guid id, CommandBarGroup group, uint sortOrder, 
            CommandBarFlags flags, bool visible, bool isCustom, bool customizable) : 
            base(id, null, (T)IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), group, sortOrder, flags, visible, isCustom, customizable)
        {
        }
    }

    public class CommandBarComboBox : CommandBarItem
    {
        protected internal override CommandBarItemDataSource ItemDataSource { get; }

        public CommandBarComboBox(Guid id, CommandComboBoxDefinition itemDefinition, CommandBarGroup group, uint sortOrder, 
            CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : this(id, itemDefinition.Name, itemDefinition, group, sortOrder, flags, true, false, true)
        {

        }

        public CommandBarComboBox(Guid id, string name, CommandComboBoxDefinition itemDefinition, CommandBarGroup group, uint sortOrder,
            CommandBarFlags flags, bool visible, bool isCustom, bool customizable)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));

            if (string.IsNullOrEmpty(name))
                name = itemDefinition.Text;

            ItemDataSource = new ComboBoxDataSource(id, name, sortOrder, group,
                itemDefinition, visible, false, isCustom, customizable, flags);
        }

        
    }

    public class MenuCommandItem : CommandBarItem
    {
        protected internal override CommandBarItemDataSource ItemDataSource { get; }
    }

    public abstract class CommandBarItem
    {
        protected internal abstract CommandBarItemDataSource ItemDataSource { get; }
    }

    [Export]
    internal class CommandBarItemFactory
    {
        [ImportMany] internal List<Lazy<CommandBarItem>> RegisteredCommandBarItems;
    }

    public enum CommandBarItemTypes
    {
        Invalid = -1,
        Separator = 0,
        Button = 1,
        SplitDropDown = 2,
        Combobox = 3,
        Menu = 4,
        MenuToolbar = 5
    }
}

