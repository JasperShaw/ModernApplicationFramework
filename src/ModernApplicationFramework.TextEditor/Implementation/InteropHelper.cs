using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal static class InteropHelper
    {
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