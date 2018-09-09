using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal abstract class SynchronizationEvent
    {
       
    }

    internal class SynchronizationManager
    {
        private readonly Queue<SynchronizationEvent> queue = new Queue<SynchronizationEvent>();
        private readonly object _mutex = new object();

        public SynchronizationManager()
        {
        }

        public void Enqueue(SynchronizationEvent newEvent)
        {
            bool flag;
            lock (_mutex)
            {
                queue.Enqueue(newEvent);
                flag = queue.Count == 1;
            }
            if (!flag)
                return;
            Execute();
        }

        private void Execute()
        {

        }
    }
}