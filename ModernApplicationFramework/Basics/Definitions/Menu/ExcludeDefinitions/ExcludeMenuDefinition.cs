namespace ModernApplicationFramework.Basics.Definitions.Menu.ExcludeDefinitions
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