using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IDummyListMenuItem
    {
        DefinitionBase CommandDefinition { get; }
        void Update(CommandHandlerWrapper commandHandler);
    }
}
