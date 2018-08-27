using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;

namespace ModernApplicationFramework.Interfaces
{
    public interface IToolbarProvider
    {
        bool HasToolbar { get; }

        CommandBarToolbar Toolbar { get; }
    }
}