using ModernApplicationFramework.Basics.CommandBar.DataSources;

namespace ModernApplicationFramework.Basics.CommandBar.Elements
{
    public abstract class CommandBarItem
    {
        public abstract CommandBarDataSource ItemDataSource { get; }
    }
}