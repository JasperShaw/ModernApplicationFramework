using System;

namespace ModernApplicationFramework.CommandBase
{
    public class CommandListDefinition : CommandDefinition
    {
        public override string IconId => null;
        public override Uri IconSource => null;
        public override bool CanShowInMenu => false;
        public override bool CanShowInToolbar => false;
        public override string Name => string.Empty;
        public override string Text => Name;
        public override string ToolTip => string.Empty;
    }
}
