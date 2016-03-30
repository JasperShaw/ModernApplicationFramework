namespace ModernApplicationFramework.Caliburn.EventArgs
{
    /// <summary>
    /// EventArgs sent during deactivation.
    /// </summary>
    public class DeactivationEventArgs : System.EventArgs
    {
        /// <summary>
        /// Indicates whether the sender was closed in addition to being deactivated.
        /// </summary>
        public bool WasClosed;
    }
}