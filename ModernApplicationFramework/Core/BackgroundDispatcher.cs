using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core
{
    public sealed class BackgroundDispatcher
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
            Validate.IsNotNullAndNotEmpty(dispatcherName, "dispatcherName");
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
            var thread1 = new Thread(ThreadProc, stackSize)
            {
                IsBackground = true,
                CurrentCulture = CultureInfo.CurrentCulture,
                CurrentUICulture = CultureInfo.CurrentUICulture
            };
            var name = _name;
            thread1.Name = name;
            var thread2 = thread1;
            thread2.SetApartmentState(ApartmentState.STA);
            var manualResetEvent = new ManualResetEvent(false);
            thread2.Start(manualResetEvent);
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

        private void ThreadProc(object arg)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ((EventWaitHandle) arg).Set();
            Dispatcher.Run();
            Dispatcher = null;
        }
    }
}