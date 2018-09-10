using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal static class InteropHelper
    {
        internal static unsafe void SetOutParameter(out int param, int value)
        {
            fixed (int* numPtr = &param)
            {
                if ((IntPtr)numPtr != IntPtr.Zero)
                    *numPtr = value;
            }
        }

        internal static unsafe void SetOutParameter(out uint param, uint value)
        {
            fixed (uint* numPtr = &param)
            {
                if ((IntPtr)numPtr != IntPtr.Zero)
                    *numPtr = value;
            }
        }

        internal static unsafe void SetOutParameter(out IntPtr param, IntPtr value)
        {
            fixed (IntPtr* numPtr = &param)
            {
                if ((IntPtr)numPtr != IntPtr.Zero)
                    *numPtr = value;
            }
        }

        internal class StringWrapper : IDisposable
        {
            private bool _disposed;

            public StringWrapper(string s)
            {
                Ptr = Marshal.StringToCoTaskMemAuto(s);
            }

            public IntPtr Ptr { get; }

            public void Dispose()
            {
                if (_disposed)
                    throw new InvalidOperationException("StringWrapper is already disposed.");
                Marshal.FreeCoTaskMem(Ptr);
                GC.SuppressFinalize(this);
                _disposed = true;
            }

            ~StringWrapper()
            {
                if (_disposed)
                    return;
                Trace.Assert(_disposed, "Dispose was not called for StringWrapper. Please use 'using' pattern for StringWrapper");
                Dispose();
            }
        }
    }
}