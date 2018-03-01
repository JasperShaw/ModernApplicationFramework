using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Native.Shell
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    class SafeModuleHandle : SafeHandle
    {
        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return Kernel32.FreeLibrary(handle);
        }
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    class ActivationContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ActivationContextSafeHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            Kernel32.ReleaseActCtx(handle);
            return true;
        }
    }
}
