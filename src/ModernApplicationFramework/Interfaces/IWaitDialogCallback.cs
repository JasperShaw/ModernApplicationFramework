namespace ModernApplicationFramework.Interfaces
{
    public interface IWaitDialogCallback
    {
        /// <summary>
        /// Called when cancellation was invoked
        /// </summary>
        void OnCanceled();
    }
}