namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public interface IEditor : IInspector
    {
        BoundPropertyDescriptor BoundPropertyDescriptor { get; set; }
    }
}