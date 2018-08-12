using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Modules.Editor.NativeMethods;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal sealed class SafeCursor : SafeHandle
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        public SafeCursor()
            : base(IntPtr.Zero, true)
        {
        }

        public SafeCursor(IntPtr hCursor)
            : base(hCursor, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return User32.DestroyCursor(handle);
        }
    }
}