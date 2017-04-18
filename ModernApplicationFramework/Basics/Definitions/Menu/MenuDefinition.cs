using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarItemDefinition
    {
        public MenuDefinition(CommandBarGroupDefinition group, uint sortOrder, string text, bool isCustom = false,
            bool isCustomizable = true)
            : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), true, false, isCustom, isCustomizable)
        {
        }

        private sealed class MenuHeaderCommandDefinition : DefinitionBase
        {
            public override string Name => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Menu;
            public override string ShortcutText => null;
        }
    }
}