using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.NativeMethods;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal sealed class SafeCursor : SafeHandle
    {
        public SafeCursor()
            : base(IntPtr.Zero, true)
        {
        }

        public SafeCursor(IntPtr hCursor)
            : base(hCursor, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            return User32.DestroyCursor(handle);
        }
    }
}