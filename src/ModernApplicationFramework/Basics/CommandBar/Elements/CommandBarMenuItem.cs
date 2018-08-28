using System;
using ModernApplicationFramework.Basics.CommandBar.DataSources;

namespace ModernApplicationFramework.Basics.CommandBar.Elements
{
    public class CommandBarMenuItem : CommandBarItem
    {
        public override CommandBarDataSource ItemDataSource { get; }

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

