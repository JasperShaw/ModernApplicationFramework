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

namespace ModernApplicationFramework.Docking.Controls
{
    internal class FocusChangeEventArgs : EventArgs
    {
        public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
        {
            GotFocusWinHandle = gotFocusWinHandle;
            LostFocusWinHandle = lostFocusWinHandle;
        }

        public IntPtr GotFocusWinHandle { get; private set; }
        public IntPtr LostFocusWinHandle { get; private set; }
    }

    internal class WindowHookHandler
    {
        //public event EventHandler<WindowActivateEventArgs> Activate;

        private readonly ReentrantFlag _insideActivateEvent = new ReentrantFlag();
        private Win32Helper.HookProc _hookProc;
        private IntPtr _windowHook;
        public event EventHandler<FocusChangeEventArgs> FocusChanged;

        public void Attach()
        {
            _hookProc = HookProc;
            _windowHook = Win32Helper.SetWindowsHookEx(
                Win32Helper.HookType.WhCbt,
                _hookProc,
                IntPtr.Zero,
                (int) Win32Helper.GetCurrentThreadId());
        }

        public void Detach()
        {
            Win32Helper.UnhookWindowsHookEx(_windowHook);
        }

        public int HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == Win32Helper.HcbtSetfocus)
            {
                FocusChanged?.Invoke(this, new FocusChangeEventArgs(wParam, lParam));
            }
            else if (code == Win32Helper.HcbtActivate)
            {
                if (_insideActivateEvent.CanEnter)
                {
                    using (_insideActivateEvent.Enter())
                    {
                        //if (Activate != null)
                        //    Activate(this, new WindowActivateEventArgs(wParam));
                    }
                }
            }


            return Win32Helper.CallNextHookEx(_windowHook, code, wParam, lParam);
        }
    }
}