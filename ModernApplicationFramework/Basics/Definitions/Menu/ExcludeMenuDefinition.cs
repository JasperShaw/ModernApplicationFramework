namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class ExcludeMenuDefinition
    {
        public ExcludeMenuDefinition ExludedMenuItemDefinitionOld { get; set; }

        public ExcludeMenuDefinition(ExcludeMenuDefinition definitionOld)
        {
            ExludedMenuItemDefinitionOld = definitionOld;
        }
    }
}