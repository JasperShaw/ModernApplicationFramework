using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class SubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public SubHeaderMenuItemDefinition(string text, CommandBarGroupDefinition group, uint sortOrder,
            bool isCustom = false, bool isCustomizable = true) : base(null, text, sortOrder, group, null, true, false, isCustom, isCustomizable)
        {
        }
    }
}