namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class ExcludeMenuDefinition
    {
        public MenuDefinition ExludedMenuItemDefinition { get; }

        public ExcludeMenuDefinition(MenuDefinition definition)
        {
            ExludedMenuItemDefinition = definition;
        }
    }
}