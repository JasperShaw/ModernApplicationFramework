using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public sealed class MenuBarDefinition : CommandBarDefinitionBase
    {
        public MenuBarDefinition(string text, uint sortOrder) : base(text, sortOrder, null, false, false)
        {
        }
    }
}
