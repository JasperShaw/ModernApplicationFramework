using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Interfaces
{
    public interface IToolbarProvider
    {
        bool HasToolbar { get; }

        CommandBarToolbar Toolbar { get; }
    }
}