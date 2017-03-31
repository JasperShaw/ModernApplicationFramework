using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;

namespace ModernApplicationFramework.Basics.Definitions.Menu.ExcludeDefinitions
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