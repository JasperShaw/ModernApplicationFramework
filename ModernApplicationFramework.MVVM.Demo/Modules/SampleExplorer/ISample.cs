using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules.SampleExplorer
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IDockingHostViewModel shell);
    }
}
