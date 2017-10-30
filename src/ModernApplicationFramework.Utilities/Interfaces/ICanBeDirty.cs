namespace ModernApplicationFramework.Utilities.Interfaces
{
    /// <summary>
    /// Indicates whether an object can have a dirty state
    /// </summary>
    public interface ICanBeDirty
    {
        IDirtyObjectManager DirtyObjectManager { get; }
    }
}
