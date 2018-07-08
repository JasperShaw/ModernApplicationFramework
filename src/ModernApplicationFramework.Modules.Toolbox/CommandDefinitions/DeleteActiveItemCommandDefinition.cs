using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    internal class DeleteActiveItemCommandDefinition : CommandDefinition<IDeleteActiveItemCommand>
    {
        private static DeleteActiveItemCommandDefinition _instance;
        public override string NameUnlocalized => null;
        public override string Text => null;
        public override string ToolTip => Text;
        public override CommandCategory Category => null;
        public override Guid Id => Guid.Empty;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
        internal static CommandDefinition Instance => _instance ?? (_instance = new DeleteActiveItemCommandDefinition());
    }
}
