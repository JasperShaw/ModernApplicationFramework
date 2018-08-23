using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core
{
    internal sealed class BackgroundDispatcher
    {
        private static readonly List<BackgroundDispatcher> SDispatchers = new List<BackgroundDispatcher>();
        private readonly string _name;

        private BackgroundDispatcher(string name, int stackSize)
        {
            _name = name;
            CreateDispatcher(stackSize);
        }

        private Dispatcher Dispatcher { get; set; }

        public static Dispatcher GetBackgroundDispatcher(string dispatcherName, int stackSize = 0)
        {
            Validate.IsNotNullAndNotEmpty(dispatcherName, nameof(dispatcherName));
            lock (SDispatchers)
            {
                foreach (var dispatcher in SDispatchers)
                    if (dispatcher._name == dispatcherName)
                        return dispatcher.Dispatcher;
                var backgroundDispatcher = new BackgroundDispatcher(dispatcherName, stackSize);
                SDispatchers.Add(backgroundDispatcher);
                return backgroundDispatcher.Dispatcher;
            }
        }

        private void CreateDispatcher(int stackSize)
        {
            Thread thread = new Thread(ThreadProc, stackSize)
            {
                IsBackground = true,
                CurrentCulture = CultureInfo.CurrentCulture,
                CurrentUICulture = CultureInfo.CurrentUICulture,
                Name = _name
            };
            thread.SetApartmentState(ApartmentState.STA);
            var manualResetEvent = new ManualResetEvent(false);
            thread.Start(manualResetEvent);
            manualResetEvent.WaitOne();
            HookEvents();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            HandleTerminationEvent();
        }

        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            HandleTerminationEvent();
        }

        private void HandleTerminationEvent()
        {
            Dispatcher.InvokeShutdown();
            UnhookEvents();
        }

        private void HookEvents()
        {
            Application.Current.Exit += OnApplicationExit;
            Dispatcher.CurrentDispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
        }

        private void UnhookEvents()
        {
            Application.Current.Exit -= OnApplicationExit;
            Dispatcher.CurrentDispatcher.ShutdownStarted -= OnDispatcherShutdownStarted;
        }

        [DebuggerStepThrough]
        private void ThreadProc(object arg)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ((EventWaitHandle) arg).Set();
            Dispatcher.Run();
            Dispatcher = null;
        }
    }
}