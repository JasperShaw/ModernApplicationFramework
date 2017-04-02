using System;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public sealed class MenuItemCommandDefinition : DefinitionBase
    {
        public override string Name => null;
        public override string Text => null;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override bool IsList => false;
        public override string ShortcutText => null;
        public override CommandControlTypes ControlType => CommandControlTypes.Menu;
    }
}