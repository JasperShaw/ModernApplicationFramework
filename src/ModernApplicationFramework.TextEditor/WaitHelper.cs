using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public static class WaitHelper
    {
        public static IWaitContext Wait(IWaitIndicator waitIndicator, string title, string message)
        {
            if (waitIndicator == null)
                return new WaitContext();
            return waitIndicator.StartWait(title, message, true);
        }

        private class WaitContext : IWaitContext
        {
            public CancellationToken CancellationToken => CancellationToken.None;

            public bool AllowCancel { get; set; }

            public string Message { get; set; }

            public void UpdateProgress()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}