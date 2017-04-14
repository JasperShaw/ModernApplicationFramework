using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CustomSubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public CustomSubHeaderMenuItemDefinition(string name,string text, CommandBarGroupDefinition group, uint sortOrder, bool isCustomizable = true) 
            : base(name, text, sortOrder, group, null, true, false, true, isCustomizable)
        {
        }
    }
}