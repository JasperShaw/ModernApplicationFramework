namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors
{
    public class CollapsibleGroupBuilder : InspectorBuilder<CollapsibleGroupBuilder>
    {
        internal CollapsibleGroupViewModel ToCollapsibleGroup(string name)
        {
            return new CollapsibleGroupViewModel(name, Inspectors);
        }
    }
}
