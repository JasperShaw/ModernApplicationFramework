/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class FocusChangeEventArgs : EventArgs
    {
        public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
        {
            GotFocusWinHandle = gotFocusWinHandle;
            LostFocusWinHandle = lostFocusWinHandle;
        }

        public IntPtr GotFocusWinHandle { get; }
        public IntPtr LostFocusWinHandle { get; }
    }

    internal class WindowHookHandler
    {
        //public event EventHandler<WindowActivateEventArgs> Activate;

        private readonly ReentrantFlag _insideActivateEvent = new ReentrantFlag();
        private NativeMethods.NativeMethods.HookProc _hookProc;
        private IntPtr _windowHook;
        public event EventHandler<FocusChangeEventArgs> FocusChanged;

        public void Attach()
        {
            _hookProc = HookProc;
            _windowHook = NativeMethods.User32.SetWindowsHookEx(
                HookType.WhCbt,
                _hookProc,
                IntPtr.Zero,
                (int)NativeMethods.Kernel32.GetCurrentThreadId());
        }

        public void Detach()
        {
            NativeMethods.User32.UnhookWindowsHookEx(_windowHook);
        }

        public int HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            switch (code)
            {
                case 9:
                    FocusChanged?.Invoke(this, new FocusChangeEventArgs(wParam, lParam));
                    break;
                case 5:
                    if (_insideActivateEvent.CanEnter)
                    {
                        using (_insideActivateEvent.Enter())
                        {
                            //if (Activate != null)
                            //    Activate(this, new WindowActivateEventArgs(wParam));
                        }
                    }
                    break;
            }


            return NativeMethods.User32.CallNextHookEx(_windowHook, code, wParam, lParam);
        }
    }
}