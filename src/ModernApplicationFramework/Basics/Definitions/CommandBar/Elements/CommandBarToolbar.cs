using System;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class CommandBarToolbar : CommandBarItem
    {
        protected internal override CommandBarDataSource ItemDataSource { get; }

        public ToolbarScope Scope { get; }

        public CommandBarToolbar(Guid id, string text, uint sortOrder, bool isCustom, Dock position) : 
            this(id, text, sortOrder, isCustom, position, ToolbarScope.MainWindow, CommandBarFlags.CommandFlagNone)
        {
            
        }

        public CommandBarToolbar(Guid id, string text, uint sortOrder, bool isCustom, Dock position,  ToolbarScope scope, CommandBarFlags flags)
        {
            Scope = scope;
            ItemDataSource = new ToolBarDataSource(id, text, sortOrder, isCustom, position, scope, flags);
        }      
    }
}
