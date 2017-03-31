namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CustomSubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public CustomSubHeaderMenuItemDefinition(string text, MenuItemGroupDefinition group, uint sortOrder) : base(text, group, sortOrder, null, true)
        {
        }
    }
}
