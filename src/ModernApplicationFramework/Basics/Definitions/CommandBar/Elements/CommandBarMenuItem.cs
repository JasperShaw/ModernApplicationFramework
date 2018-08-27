using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public class CommandBarMenuItem : CommandBarItem
    {
        protected internal override CommandBarDataSource ItemDataSource { get; }

        public CommandBarMenuItem(Guid id, string text, CommandBarGroup group, uint sortOrder) : 
            this(id, text, group, sortOrder, false, CommandBarFlags.CommandFlagNone)
        {
            
        }

        public CommandBarMenuItem(Guid id, string text, CommandBarGroup group, uint sortOrder, bool isCustom, CommandBarFlags flags)
        {
            ItemDataSource = new MenuDataSource(id, text, group, sortOrder, isCustom, flags);
        }
    }
}

