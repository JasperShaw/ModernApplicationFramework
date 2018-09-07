using System;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal abstract class InvokableBase : IMafInvokablePrivate
    {
        protected abstract void InvokeMethod();

        public int Invoke()
        {
            VerifyAccess();
            try
            {
                InvokeMethod();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            return 0;
        }

        public Exception Exception { get; private set; }

        private static void VerifyAccess()
        {
            if (!ThreadHelper.CheckAccess())
                throw new InvalidOperationException();
        }
    }
}