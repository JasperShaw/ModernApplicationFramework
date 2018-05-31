using ModernApplicationFramework.Docking;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <summary>
    /// A control that holds a <see cref="Docking.DockingManager"/>
    /// </summary>
    public interface IDockingHost
    {
        DockingManager DockingManager { get; }

        /// <summary>
        /// Updates all floating windows.
        /// </summary>
        void UpdateFloatingWindows();
    }
}