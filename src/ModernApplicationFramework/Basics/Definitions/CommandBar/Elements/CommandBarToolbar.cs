using System;
using System.Windows.Controls;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class CommandBarToolbar : CommandBarItem
    {
        protected internal override CommandBarDataSource ItemDataSource { get; }

        public ToolbarScope Scope { get; }

        public CommandBarToolbar(Guid id, string text, uint bandIndex, bool isCustom, Dock position) : 
            this(id, text, 0, bandIndex, isCustom, position, ToolbarScope.MainWindow, CommandBarFlags.CommandFlagNone)
        {
            
        }

        public CommandBarToolbar(Guid id, string text, uint placementSlot, uint bandIndex, bool isCustom, Dock position,  ToolbarScope scope, CommandBarFlags flags)
        {
            Scope = scope;
            ItemDataSource =
                new ToolBarDataSource(id, text, placementSlot, bandIndex, isCustom, position, scope, flags);
        }      
    }
}
