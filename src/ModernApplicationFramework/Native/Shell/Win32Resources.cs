using System;
using System.Text;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Native.Shell
{
    internal class Win32Resources : IDisposable
    {
        private readonly SafeModuleHandle _moduleHandle;
        private const int BufferSize = 500;

        public Win32Resources(string module)
        {
            _moduleHandle = Kernel32.LoadLibraryEx(module, IntPtr.Zero, LoadLibraryExFlags.LoadLibraryAsDatafile);
            if (_moduleHandle.IsInvalid)
                throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
        }

        public string LoadString(uint id)
        {
            CheckDisposed();

            StringBuilder buffer = new StringBuilder(BufferSize);
            if (User32.LoadString(_moduleHandle, id, buffer, buffer.Capacity + 1) == 0)
                throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            return buffer.ToString();
        }

        public string FormatString(uint id, params string[] args)
        {
            CheckDisposed();

            IntPtr buffer = IntPtr.Zero;
            string source = LoadString(id);

            // For some reason FORMAT_MESSAGE_FROM_HMODULE doesn't work so we use this way.
            FormatMessageFlags flags = FormatMessageFlags.FormatMessageAllocateBuffer | FormatMessageFlags.FormatMessageArgumentArray | FormatMessageFlags.FormatMessageFromString;

            IntPtr sourcePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAuto(source);
            try
            {
                if (Kernel32.FormatMessage(flags, sourcePtr, id, 0, ref buffer, 0, args) == 0)
                    throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(sourcePtr);
            }

            string result = System.Runtime.InteropServices.Marshal.PtrToStringAuto(buffer);
            // FreeHGlobal calls LocalFree
            System.Runtime.InteropServices.Marshal.FreeHGlobal(buffer);

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _moduleHandle.Dispose();
        }

        private void CheckDisposed()
        {
            if (_moduleHandle.IsClosed)
            {
                throw new ObjectDisposedException("Win32Resources");
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion    
    }
}
