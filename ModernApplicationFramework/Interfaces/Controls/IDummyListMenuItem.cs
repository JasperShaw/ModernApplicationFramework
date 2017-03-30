using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IDummyListMenuItem
    {
        CommandBarDefinitionBase CommandBarItemDefinition { get; }
        void Update(CommandHandlerWrapper commandHandler);
    }
}
