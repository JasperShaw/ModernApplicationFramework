using System;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    internal abstract class InvokableBase : IMafInvokablePrivate
    {
        public Exception Exception { get; private set; }

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

        protected abstract void InvokeMethod();

        private static void VerifyAccess()
        {
            if (!ThreadHelper.CheckAccess())
                throw new InvalidOperationException();
        }
    }
}