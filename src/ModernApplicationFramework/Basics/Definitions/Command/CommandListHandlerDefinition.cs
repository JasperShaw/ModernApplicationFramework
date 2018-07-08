using System;
using System.Collections.Generic;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public sealed class CommandListHandlerDefinition : CommandDefinition
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => string.Empty;
        public override string NameUnlocalized => string.Empty;

        public override string Text { get; }
        public override string ToolTip => string.Empty;

        public override CommandCategory Category => null;
        public override Guid Id => Guid.Empty;

        public CommandListHandlerDefinition(string text, ICommandDefinitionCommand command)
        {
            Text = text;
            Command = command;
        }
    }
}
