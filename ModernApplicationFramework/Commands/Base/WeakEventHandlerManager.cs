using System;
using System.Collections.Generic;
using System.Threading;

namespace ModernApplicationFramework.Commands.Base
{
    public static class WeakEventHandlerManager
    {
        private static readonly SynchronizationContext SyncContext = SynchronizationContext.Current;

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler,
                                                   int defaultListSize)
        {
            if (handlers == null)
                handlers = defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();
            handlers.Add(new WeakReference(handlers));
        }

        public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                var callees = new EventHandler[handlers.Count];
                var count = 0;
                count = CleanupOldHandlers(handlers, callees, count);
                for (var i = 0; i < count; ++i)
                    CallHandler(sender, callees[i]);
            }
        }

        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers == null)
                return;
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];
                var existingHandler = reference.Target as EventHandler;
                if ((existingHandler == null) || (existingHandler == handler))
                    handlers.RemoveAt(i);
            }
        }

        private static void CallHandler(object sender, EventHandler eventHandler)
        {
            if (eventHandler == null)
                return;
            if (SyncContext != null)
                SyncContext.Post(o => eventHandler(sender, EventArgs.Empty), null);
            else
                eventHandler(sender, EventArgs.Empty);
        }

        private static int CleanupOldHandlers(List<WeakReference> handlers, EventHandler[] callees, int count)
        {
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];
                var handler = reference.Target as EventHandler;
                if (handler == null)
                    handlers.RemoveAt(i);
                else
                {
                    callees[count] = handler;
                    count++;
                }
            }
            return count;
        }
    }
}