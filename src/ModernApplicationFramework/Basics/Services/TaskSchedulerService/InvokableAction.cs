using System;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal class InvokableAction : InvokableBase
    {
        private readonly Action _a;
        private readonly MafExecutionContextTrackerHelper.CapturedContext _context;

        public InvokableAction(Action a, bool captureContext = false)
        {
            _a = a;
            if (!captureContext)
                return;
            _context = MafExecutionContextTrackerHelper.CaptureCurrentContext();
        }

        protected override void InvokeMethod()
        {
            if (_context != null)
                _context.ExecuteUnderContext(_a);
            else
                _a();
        }
    }
}