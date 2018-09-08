using System;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal static class MafExecutionContextTrackerHelper
    {
        private static IMafExecutionContextTracker _instance;

        public static IMafExecutionContextTracker Instance =>
            _instance ?? (_instance = new MafExecutionContextTracker());

        public static CapturedContext CaptureCurrentContext()
        {
            return new CapturedContext();
        }

        public static uint GetCurrentContext()
        {
            if (Instance != null) return Instance.GetCurrentContext();
            return 0;
        }

        public class CapturedContext : IDisposable
        {
            private readonly uint _capturedContext;

            internal CapturedContext()
            {
                _capturedContext = 0U;
                if (Instance == null)
                    return;
                _capturedContext = Instance.GetCurrentContext();
            }

            public void Dispose()
            {
                Instance?.ReleaseContext(_capturedContext);
            }

            public void ExecuteUnderContext(Action t)
            {
                Instance?.PushContext(_capturedContext);
                t();
                Instance?.PopContext(_capturedContext);
            }
        }
    }
}