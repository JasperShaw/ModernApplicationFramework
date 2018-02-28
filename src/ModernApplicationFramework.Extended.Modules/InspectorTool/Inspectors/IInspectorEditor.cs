namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public interface IInspectorEditor : IInspector
    {
        BoundPropertyDescriptor BoundPropertyDescriptor { get; set; }
    }
}