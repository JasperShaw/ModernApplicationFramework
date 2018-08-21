using System;
using ModernApplicationFramework.Basics.Definitions.Command;
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
        internal static CommandDefinition Instance => _instance ?? (_instance = new DeleteActiveItemCommandDefinition());
    }
}
