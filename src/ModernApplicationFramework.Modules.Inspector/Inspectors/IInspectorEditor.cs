namespace ModernApplicationFramework.Modules.Inspector.Inspectors
{
    public interface IInspectorEditor : IInspector
    {
        BoundPropertyDescriptor BoundPropertyDescriptor { get; set; }
    }
}