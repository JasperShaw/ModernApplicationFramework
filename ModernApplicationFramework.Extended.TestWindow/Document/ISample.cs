using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.TestWindow.Document
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IDockingHostViewModel shell);
    }
}
