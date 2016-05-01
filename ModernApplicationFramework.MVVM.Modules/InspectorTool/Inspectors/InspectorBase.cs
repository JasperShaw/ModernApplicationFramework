using Caliburn.Micro;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors
{
    public abstract class InspectorBase : PropertyChangedBase, IInspector
    {
        public abstract string Name { get; }
        public abstract bool IsReadOnly { get; }
    }
}
