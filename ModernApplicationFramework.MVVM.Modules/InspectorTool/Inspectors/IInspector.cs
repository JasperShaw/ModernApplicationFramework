namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors
{
    public interface IInspector
    {
        string Name { get; }
        bool IsReadOnly { get; }
    }
}
