namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CustomSubHeaderMenuItemDefinition : MenuItemDefinition
    {
        public CustomSubHeaderMenuItemDefinition(string name,string text, MenuItemGroupDefinition group, uint sortOrder) 
            : base(name, text, sortOrder, group, null, true, false, true)
        {
        }
    }
}