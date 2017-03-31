namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class SubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public SubHeaderMenuItemDefinition(string text, MenuItemGroupDefinition group, uint sortOrder,
            bool isCustom = false) : base(text, sortOrder, group, null, true, false, isCustom)
        {
        }
    }
}