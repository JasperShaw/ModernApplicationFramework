using System;
using Caliburn.Micro;
using Action = System.Action;

namespace ModernApplicationFramework.Extended.UIContext
{
    internal static class ExecutionContextTrackerHelper
    {
        private static IExecutionContextTracker _instance;

        public static IExecutionContextTracker Instance =>
            _instance ?? (_instance = IoC.Get<IExecutionContextTracker>());

        public static uint GetCurrentContext()
        {
            return Instance?.GetCurrentContext() ?? 0;
        }

        public static CapturedContext CaptureCurrentContext()
        {
            return new CapturedContext();
        }


        public class CapturedContext : IDisposable
        {
            private readonly uint _capturedContext;

            internal CapturedContext()
            {
                _capturedContext = 0;
                if (Instance == null)
                    return;
                _capturedContext = Instance.GetCurrentContext();
            }

            public void ExecuteUnderContext(Action t)
            {
                Instance?.PushContext(_capturedContext);
                t();
                Instance?.PopContext(_capturedContext);
            }

            public void Dispose()
            {
                Instance?.ReleaseContext(_capturedContext);
            }
        }
    }
}
