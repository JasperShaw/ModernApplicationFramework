using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.CommandFocus
{
    internal class CommandRestoreFocusScope : RestoreFocusScope
    {
        private readonly IntPtr _expectedFocusWindowBeforeRestore;

        private bool ShouldPerformRestoration => RestoreFocusWindow == IntPtr.Zero ||
                                                 _expectedFocusWindowBeforeRestore == User32.GetFocus() ||
                                                 _expectedFocusWindowBeforeRestore == IntPtr.Zero;

        public CommandRestoreFocusScope(IInputElement restoreFocus, IntPtr restoreFocusWindow,
            IntPtr expectedFocusWindowBeforeRestore)
            : base(restoreFocus, restoreFocusWindow)
        {
            _expectedFocusWindowBeforeRestore = expectedFocusWindowBeforeRestore;
        }

        public override void PerformRestoration()
        {
            if (!ShouldPerformRestoration)
                return;
            base.PerformRestoration();
            if (RestoreFocus != null)
                return;
            Keyboard.ClearFocus();
        }

        protected override bool IsRestorationTargetValid()
        {
            return true;
        }
    }
}