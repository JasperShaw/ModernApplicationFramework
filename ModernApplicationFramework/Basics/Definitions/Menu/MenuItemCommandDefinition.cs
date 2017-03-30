using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class MenuItemCommandDefinition : DefinitionBase
    {
        public override string Name => null;
        public override string Text => null;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override bool IsList => false;
        public override CommandControlTypes ControlType => CommandControlTypes.Menu;
    }
}
