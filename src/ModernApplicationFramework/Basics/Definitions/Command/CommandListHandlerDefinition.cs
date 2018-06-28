using System;
using System.Collections.Generic;
using System.Windows.Input;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public sealed class CommandListHandlerDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => string.Empty;
        public override string NameUnlocalized => string.Empty;

        public override string Text { get; }
        public override string ToolTip => string.Empty;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => null;
        public override Guid Id => Guid.Empty;

        public CommandListHandlerDefinition(string text, ICommand command)
        {
            Text = text;
            Command = command;
        }
    }
}
