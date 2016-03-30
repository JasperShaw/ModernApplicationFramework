using ModernApplicationFramework.Caliburn.Interfaces;

namespace ModernApplicationFramework.Caliburn.Extensions
{
    /// <summary>
    /// The event args for the <see cref="IViewAware.ViewAttached"/> event.
    /// </summary>
    public class ViewAttachedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The context.
        /// </summary>
        public object Context;

        /// <summary>
        /// The view.
        /// </summary>
        public object View;
    }
}