namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public class SubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public SubHeaderMenuItemDefinition(string text, MenuItemGroupDefinition group, uint sortOrder,
            bool isCustom = false) : base(null, text, sortOrder, group, null, true, false, isCustom)
        {
        }
    }
}