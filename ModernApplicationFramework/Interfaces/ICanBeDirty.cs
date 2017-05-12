using ModernApplicationFramework.Basics;

namespace ModernApplicationFramework.Interfaces
{
    public interface ICanBeDirty
    {
        IDirtyObjectManager DirtyObjectManager { get; }
    }
}
