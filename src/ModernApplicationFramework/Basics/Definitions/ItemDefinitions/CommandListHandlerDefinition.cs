using System;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.Definitions.ItemDefinitions
{
    public sealed class CommandListHandlerDefinition : CommandDefinition
    {
        public override string Name => string.Empty;
        public override string NameUnlocalized => string.Empty;

        public override string Text { get; }
        public override string ToolTip => string.Empty;

        public override CommandBarCategory Category => null;
        public override Guid Id => Guid.Empty;

        public CommandListHandlerDefinition(string text, ICommandDefinitionCommand command) : base(command)
        {
            Text = text;
        }
    }
}
