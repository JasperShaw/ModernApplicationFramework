using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Interfaces
{
    public interface IToolbarProvider
    {
        bool HasToolbar { get; }

        ToolbarDefinition Toolbar { get; }
    }
}