using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class SubHeaderMenuItemDefinition : CommandBarItemDefinition
    {
        public SubHeaderMenuItemDefinition(string text, CommandBarGroupDefinition group, uint sortOrder,
            bool isCustom = false, bool isCustomizable = true) : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), true, false,
            isCustom, isCustomizable)
        {
        }
    }
}