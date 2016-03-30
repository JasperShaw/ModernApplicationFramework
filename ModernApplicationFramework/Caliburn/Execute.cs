using System.Threading.Tasks;
using Action = System.Action;

namespace ModernApplicationFramework.Caliburn
{
    /// <summary>
    ///   Enables easy marshalling of code to the UI thread.
    /// </summary>
    public static class Execute
    {
        /// <summary>
        ///   Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public static bool InDesignMode => PlatformProvider.PlatformProvider.Current.InDesignMode;

        /// <summary>
        ///   Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void BeginOnUiThread(this Action action)
        {
            PlatformProvider.PlatformProvider.Current.BeginOnUiThread(action);
        }

        /// <summary>
        ///   Executes the action on the UI thread.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static void OnUiThread(this Action action)
        {
            PlatformProvider.PlatformProvider.Current.OnUiThread(action);
        }

        /// <summary>
        ///   Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static Task OnUiThreadAsync(this Action action)
        {
            return PlatformProvider.PlatformProvider.Current.OnUiThreadAsync(action);
        }
    }
}