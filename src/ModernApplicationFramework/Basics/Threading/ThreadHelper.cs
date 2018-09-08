using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Threading;
using Action = System.Action;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Provides a generic dispatcher helper to ensure that a method is invoked on the application's main thread.
    /// </summary>
    public abstract class ThreadHelper
    {
        private static ThreadHelper _generic;
        private static JoinableTaskContext _joinableTaskContextCache;
        private static Dispatcher _uiThreadDispatcher;

        /// <summary>
        ///     Gets a generic <see cref="ThreadHelper" />
        /// </summary>
        public static ThreadHelper Generic => _generic ?? (_generic = new GenericThreadHelper());

        /// <summary>
        ///     Gets the singleton <see cref="ModernApplicationFramework.Threading.JoinableTaskContext" /> instance for the
        ///     application
        /// </summary>
        public static JoinableTaskContext JoinableTaskContext => _joinableTaskContextCache ??
                                                                 (_joinableTaskContextCache =
                                                                     MafTaskHelper.ServiceInstance
                                                                         .GetAsyncTaskContext());

        /// <summary>
        ///     Gets the joinable task factory for the application.
        /// </summary>
        public static JoinableTaskFactory JoinableTaskFactory => JoinableTaskContext.Factory;

        private static Dispatcher DispatcherForUiThread
        {
            get
            {
                if (_uiThreadDispatcher == null && Application.Current != null)
                    _uiThreadDispatcher = Application.Current.Dispatcher;
                return _uiThreadDispatcher;
            }
        }


        static ThreadHelper()
        {
            SetUiThread();
        }

        /// <summary>
        ///     Determines whether the call is being made on the UI thread.
        /// </summary>
        /// <returns>Returns <see langword="true" /> if the call is on the UI thread, otherwise returns <see langword="false" />.</returns>
        public static bool CheckAccess()
        {
            var dispatcherForUiThread = DispatcherForUiThread;
            return dispatcherForUiThread != null && dispatcherForUiThread.CheckAccess();
        }

        /// <summary>
        ///     Determines whether the call is being made on the UI thread, and throws COMException(RPC_E_WRONG_THREAD) if it is
        ///     not.
        /// </summary>
        /// <param name="callerMemberName">The optional name of caller if a Debug Assert is desired if not on the UI thread.</param>
        /// <exception cref="COMException">Thrown with RPC_E_WRONG_THREAD when called on any thread other than the main UI thread.</exception>
        public static void ThrowIfNotOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (!CheckAccess())
                throw new COMException(string.Format(CultureInfo.CurrentCulture, "{0} must be called on the UI thread.",
                    new object[]
                    {
                        callerMemberName
                    }), -2147417842);
        }

        /// <summary>
        ///     Determines whether the call is being made on the UI thread ,and throws COMException(RPC_E_WRONG_THREAD) if it is.
        /// </summary>
        /// <param name="callerMemberName">The optional name of caller if a Debug Assert is desired if on the UI thread.</param>
        /// <exception cref="COMException">Thrown with RPC_E_WRONG_THREAD when called on any thread other than the main UI thread.</exception>
        public static void ThrowIfOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (CheckAccess())
                throw new COMException(string.Format(CultureInfo.CurrentCulture,
                    "{0} must be called on a background thread.", new object[]
                    {
                        callerMemberName
                    }), -2147417842);
        }

        /// <summary>
        ///     Schedules an action for execution on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public void BeginInvoke(Action action)
        {
            BeginInvoke(DispatcherPriority.Normal, action);
        }

        /// <summary>
        ///     Schedules an action for execution on the UI thread asynchronously.
        /// </summary>
        /// <param name="priority">The priority at which to run the action.</param>
        /// <param name="action">The action to run.</param>
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
                            if (onRpcCallFailed())
                                break;
                    }

                    await Task.Delay(100);
                }
            });
        }

        internal static void SetUiThread()
        {
            _uiThreadDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected abstract IDisposable GetInvocationWrapper();
    }
}