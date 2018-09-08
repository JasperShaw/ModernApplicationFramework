using System;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class TaskCreatedEventArgs : EventArgs
    {
        public MafTask NewTask { get; }

        public TaskCreatedEventArgs(MafTask newTask)
        {
            NewTask = newTask;
        }
    }
}