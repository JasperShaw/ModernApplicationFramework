using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition : MenuItemDefinition
    {
        public CommandMenuItemDefinition(DefinitionBase commandDefinition, bool isCustom = false)
            : base(null, UInt32.MinValue, null, commandDefinition, true, false, isCustom)
        {
            DisplayName = CommandDefinition.Text;
        }
    }
}