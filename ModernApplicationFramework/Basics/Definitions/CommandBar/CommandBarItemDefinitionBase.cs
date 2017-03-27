using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinitionBase : CommandBarDefinitionBase
    {
        public abstract DefinitionBase CommandDefinition { get; set; }
    }
}
