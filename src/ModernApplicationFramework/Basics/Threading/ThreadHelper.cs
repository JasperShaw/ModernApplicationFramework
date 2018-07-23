using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Action = System.Action;

namespace ModernApplicationFramework.Basics.Threading
{
    public abstract class ThreadHelper
    {
        private static ThreadHelper _generic;
        private static Dispatcher _uiThreadDispatcher;

        public static ThreadHelper Generic => _generic ?? (_generic = new GenericThreadHelper());


        static ThreadHelper()
        {
            SetUiThread();
        }

        private static Dispatcher DispatcherForUiThread
        {
            get
            {
                if (_uiThreadDispatcher == null && Application.Current != null)
                    _uiThreadDispatcher = Application.Current.Dispatcher;
                return _uiThreadDispatcher;
            }
        }

        public void BeginInvoke(Action action)
        {
            BeginInvoke(DispatcherPriority.Normal, action);
        }

        public void BeginInvoke(DispatcherPriority priority, Action action)
        {
            var dispatcherForUiThread = DispatcherForUiThread;
            if (dispatcherForUiThread == null)
                throw new InvalidOperationException();
            dispatcherForUiThread.BeginInvoke(priority, action);
        }

        public void Invoke(Action action)
        {
            using (GetInvocationWrapper())
            {
                if (CheckAccess())
                    action();
                else
                    action.OnUIThread();
            }
        }

        public async Task InvokeAsync(Action executeAction, Func<bool> onRpcCallFailed = null)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        Invoke(executeAction);
                        break;
                    }
                    catch (InvalidComObjectException)
                    {
                        break;
                    }
                    catch (COMException ex)
                    {
                        if (ex.HResult != -2147417856)
                            throw;
                        if (onRpcCallFailed != null)
                        {
                            if (onRpcCallFailed())
                                break;
                        }
                    }
                    await Task.Delay(100);
                }
            });
        }


        protected abstract IDisposable GetInvocationWrapper();

        internal static void SetUiThread()
        {
            _uiThreadDispatcher = Dispatcher.CurrentDispatcher;
        }

        public static bool CheckAccess()
        {
            var dispatcherForUiThread = DispatcherForUiThread;
            return dispatcherForUiThread != null && dispatcherForUiThread.CheckAccess();
        }

        public static void ThrowIfNotOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (!CheckAccess())
                throw new COMException(string.Format(CultureInfo.CurrentCulture, "{0} must be called on the UI thread.", new object[]
                {
                    callerMemberName
                }), -2147417842);
        }
    }

    internal class GenericThreadHelper : ThreadHelper
    {
        protected override IDisposable GetInvocationWrapper()
        {
            return null;
        }
    }
}
