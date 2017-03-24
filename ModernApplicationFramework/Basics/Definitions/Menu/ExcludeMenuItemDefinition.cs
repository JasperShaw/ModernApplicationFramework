namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class ExcludeMenuItemDefinition
    {
        public MenuItemDefinition MenuItemDefinitionToExclude { get; }

        public ExcludeMenuItemDefinition(MenuItemDefinition menuItemDefinition)
        {
            MenuItemDefinitionToExclude = menuItemDefinition;
        }
    }
}