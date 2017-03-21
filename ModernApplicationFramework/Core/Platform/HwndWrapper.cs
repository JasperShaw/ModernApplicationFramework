﻿using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Platform.Structs;

namespace ModernApplicationFramework.Core.Platform
{
    public abstract class HwndWrapper : DisposableObject
    {
        private bool _isHandleCreationAllowed = true;
        private IntPtr _handle;
        private ushort _wndClassAtom;
        private Delegate _wndProc;
        private static long _failedDestroyWindows;
        private static int _lastDestroyWindowError;

        protected ushort GetWindowClassAtom()
        {
            if (_wndClassAtom == 0)
                _wndClassAtom = CreateWindowClassCore();
            return _wndClassAtom;
        }

        public IntPtr Handle
        {
            get
            {
                EnsureHandle();
                return _handle;
            }
        }

        public bool IsWindowSubclassed { get; set; }

        [CLSCompliant(false)]
        protected virtual ushort CreateWindowClassCore()
        {
            return RegisterClass(Guid.NewGuid().ToString());
        }

        protected virtual void DestroyWindowClassCore()
        {
            if (_wndClassAtom == 0)
                return;
            User32.UnregisterClass(new IntPtr(_wndClassAtom), Kernel32.GetModuleHandle(null));
            _wndClassAtom = 0;
        }

        [CLSCompliant(false)]
        protected ushort RegisterClass(string className)
        {
            var lpWndClass = new WndClass
            {
                cbClsExtra = 0,
                cbWndExtra = 0,
                hbrBackground = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                lpfnWndProc = _wndProc = new NativeMethods.NativeMethods.WndProc(WndProc),
                lpszClassName = className,
                lpszMenuName = null,
                style = 0U
            };
            return User32.RegisterClass(ref lpWndClass);
        }

        private void SubclassWndProc()
        {
            _wndProc = new NativeMethods.NativeMethods.WndProc(WndProc);
            NativeMethods.NativeMethods.SetWindowLong(_handle, -4, Marshal.GetFunctionPointerForDelegate(_wndProc));
        }

        protected abstract IntPtr CreateWindowCore();

        protected virtual void DestroyWindowCore()
        {
            if (!(_handle != IntPtr.Zero))
                return;
            if (!User32.DestroyWindow(_handle))
            {
                _lastDestroyWindowError = Marshal.GetLastWin32Error();
                ++_failedDestroyWindows;
            }
            _handle = IntPtr.Zero;
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        public void EnsureHandle()
        {
            if (!(_handle == IntPtr.Zero))
                return;
            if (!_isHandleCreationAllowed)
            {
                throw new NotSupportedException();
            }
            _isHandleCreationAllowed = false;
            _handle = CreateWindowCore();
            if (!IsWindowSubclassed)
                return;
            SubclassWndProc();
        }

        protected override void DisposeNativeResources()
        {
            _isHandleCreationAllowed = false;
            DestroyWindowCore();
            DestroyWindowClassCore();
        }
    }
}
