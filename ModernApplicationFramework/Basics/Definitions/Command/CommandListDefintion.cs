using System;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model used for application commands providing a list of definitions
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase" />
    public abstract class CommandListDefinition : DefinitionBase
    {
        public override string IconId => null;
        public sealed override bool IsList => true;
        public override CommandControlTypes ControlType => CommandControlTypes.Button;
        public override Uri IconSource => null;
        public override string Name => string.Empty;
        public override string Text => Name;
        public override string ToolTip => string.Empty;
        public override string ShortcutText => null;
    }
}