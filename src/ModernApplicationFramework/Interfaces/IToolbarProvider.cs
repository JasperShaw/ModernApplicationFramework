using ModernApplicationFramework.Basics.CommandBar.Elements;

namespace ModernApplicationFramework.Interfaces
{
    public interface IToolbarProvider
    {
        bool HasToolbar { get; }

        CommandBarToolbar Toolbar { get; }
    }
}