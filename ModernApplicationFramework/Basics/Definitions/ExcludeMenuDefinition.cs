namespace ModernApplicationFramework.Basics.Definitions
{
    public class ExcludeMenuDefinition
    {
        public MenuItemDefinition ExludedMenuItemDefinition { get; }

        public ExcludeMenuDefinition(MenuItemDefinition definition)
        {
            ExludedMenuItemDefinition = definition;
        }
    }
}
