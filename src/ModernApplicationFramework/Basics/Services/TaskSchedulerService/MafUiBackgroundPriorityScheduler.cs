﻿using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafUiBackgroundPriorityScheduler : MafUiThreadBlockableTaskScheduler
    {
        private readonly TimeSpan _defaultTimerInterval = TimeSpan.FromMilliseconds(300.0);
        private readonly DispatcherTimer _timerObject;
        private readonly TimeSpan _yieldInterval = TimeSpan.FromMilliseconds(1.0);

        public override MafTaskRunContext SchedulerContext => MafTaskRunContext.UiThreadBackgroundPriority;

        public MafUiBackgroundPriorityScheduler()
        {
            _timerObject = new DispatcherTimer {Interval = _defaultTimerInterval};
            _timerObject.Tick += OnTimerTick;
        }

        internal void ProcessQueue(string caller)
        {
            if (!TryGetTicksSinceLastInput(out var ticks))
                ticks = 300;
            var flag = true;
            if (ticks < 300)
            {
                _timerObject.Interval = TimeSpan.FromMilliseconds(300 - ticks);
            }
            else if (IsInputPending(133))
            {
                _timerObject.Interval = _defaultTimerInterval;
            }
            else if (IsInputPending(2))
            {
                _timerObject.Interval = _yieldInterval;
            }
            else if (IsSystemCommandPending())
            {
                _timerObject.Interval = _yieldInterval;
            }
            else
            {
                _timerObject.Interval = _yieldInterval;
                if (!DoOneTask(out _)) flag = false;
            }

            _timerObject.IsEnabled = flag;
        }

        protected override void OnTaskQueued(Task task)
        {
            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.Render, () => ProcessQueue("DispatchAfterTaskQueue"));
        }

        private static bool IsInputPending(int dwQsFlags)
        {
            return User32.MsgWaitForMultipleObjectsEx(0, null, 0, dwQsFlags, 4) == 0;
        }

        private static bool IsSystemCommandPending()
        {
            return User32.PeekMessage(out _, IntPtr.Zero, 274U, 274U, 0U);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            ProcessQueue("TimerTick");
        }

        private bool TryGetTicksSinceLastInput(out int ticks)
        {
            ticks = 0;
            return false;
        }
    }
}