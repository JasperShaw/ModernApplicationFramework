namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public interface IInspector
    {
        string Name { get; }
        bool IsReadOnly { get; }
    }
}