namespace ModernApplicationFramework.MVVM.Modules.InspectorTool
{
    public class InspectableObjectBuilder : InspectorBuilder<InspectableObjectBuilder>
    {
        public InspectableObject ToInspectableObject()
        {
            return new InspectableObject(Inspectors);
        }
    }
}
