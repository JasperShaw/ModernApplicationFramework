using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.ItemDefinitions
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model used for application commands providing a list of definitions
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class ListCommandDefinition : CommandItemDefinitionBase
    {
        public sealed override bool IsList => true;
        public override CommandControlTypes ControlType => CommandControlTypes.Button;
        public override string Name => string.Empty;
        public override string Text => Name;
        public override string ToolTip => string.Empty;
    }
}