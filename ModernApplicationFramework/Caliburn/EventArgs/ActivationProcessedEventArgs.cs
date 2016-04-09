using ModernApplicationFramework.Caliburn.Conductor;

namespace ModernApplicationFramework.Caliburn.EventArgs
{
    /// <summary>
    ///     Contains details about the success or failure of an item's activation through an <see cref="IConductor" />.
    /// </summary>
    public class ActivationProcessedEventArgs : System.EventArgs
    {
        /// <summary>
        ///     The item whose activation was processed.
        /// </summary>
        public object Item;

        /// <summary>
        ///     Gets or sets a value indicating whether the activation was a success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success;
    }
}