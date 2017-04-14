using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarItemDefinition
    {
        public MenuBarDefinition MenuBar { get; }

        public MenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string text, bool isCustom = false,
            bool isCustomizable = true)
            : base(text, sortOrder, null, new MenuHeaderCommandDefinition(), true, false, isCustom, isCustomizable)
        {
            MenuBar = menuBar;
        }
    }
}