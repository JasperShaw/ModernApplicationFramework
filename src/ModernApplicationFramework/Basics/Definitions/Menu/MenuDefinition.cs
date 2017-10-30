using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    /// <inheritdoc />
    /// <summary>
    /// A command bar definition that specifies a menu item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public class MenuDefinition : CommandBarItemDefinition
    {
        public MenuDefinition(CommandBarGroupDefinition group, uint sortOrder, string text, bool isCustom = false,
            bool isCustomizable = true)
            : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), true, false, isCustom, isCustomizable)
        {
        }

        private sealed class MenuHeaderCommandDefinition : CommandDefinitionBase
        {
            public override string Name => null;
            public override string NameUnlocalized => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Menu;
        }
    }
}