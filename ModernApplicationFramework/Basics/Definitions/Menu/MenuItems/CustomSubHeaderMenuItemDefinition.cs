using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CustomSubHeaderMenuItemDefinition : CommandBarItemDefinition
    {
        public CustomSubHeaderMenuItemDefinition(string text, CommandBarGroupDefinition group, uint sortOrder, bool isCustomizable = true) 
            : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), true, false, true, isCustomizable)
        {
        }
    }
}