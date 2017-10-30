using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IDockingHostViewModel shell);
    }
}
