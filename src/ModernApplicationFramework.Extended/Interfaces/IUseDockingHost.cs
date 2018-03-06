namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <summary>
    /// This instance contains a <see cref="IDockingHostViewModel"/>
    /// </summary>
    public interface IUseDockingHost
    {
        /// <summary>
        /// The docking host view model
        /// </summary>
        IDockingHostViewModel DockingHost { get; }
    }
}