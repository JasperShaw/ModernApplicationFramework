/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace ModernApplicationFramework.Core.Standard
{
    internal sealed class MessageWindow : DispatcherObject, IDisposable
    {
        // Alias this to a static so the wrapper doesn't get GC'd
        private static readonly WndProc SWndProc = _WndProc;

        private static readonly Dictionary<IntPtr, MessageWindow> SWindowLookup =
            new Dictionary<IntPtr, MessageWindow>();

        private string _className;
        private bool _isDisposed;

        private readonly WndProc _wndProcCallback;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public MessageWindow(CS classStyle, WS style, WS_EX exStyle, Rect location, string name, WndProc callback)
        {
            // A null callback means just use DefWindowProc.
            _wndProcCallback = callback;
            _className = "MessageWindowClass+" + Guid.NewGuid();

            var wc = new WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = classStyle,
                lpfnWndProc = SWndProc,
                hInstance = NativeMethods.GetModuleHandle(null),
                hbrBackground = NativeMethods.GetStockObject(StockObject.NULL_BRUSH),
                lpszMenuName = "",
                lpszClassName = _className
            };

            NativeMethods.RegisterClassEx(ref wc);

            var gcHandle = default(GCHandle);
            try
            {
                gcHandle = GCHandle.Alloc(this);
                var pinnedThisPtr = (IntPtr) gcHandle;

                Handle = NativeMethods.CreateWindowEx(
                    exStyle,
                    _className,
                    name,
                    style,
                    (int) location.X,
                    (int) location.Y,
                    (int) location.Width,
                    (int) location.Height,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    pinnedThisPtr);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        ~MessageWindow()
        {
            _Dispose(false);
        }

        public IntPtr Handle { get; private set; }

        public void Dispose()
        {
            _Dispose(false);
            GC.SuppressFinalize(this);
        }

        private static object _DestroyWindow(IntPtr hwnd, string className)
        {
            Utility.SafeDestroyWindow(ref hwnd);
            NativeMethods.UnregisterClass(className, NativeMethods.GetModuleHandle(null));
            return null;
        }

        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        private static IntPtr _WndProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam)
        {
            MessageWindow hwndWrapper;

            if (msg == WM.CREATE)
            {
                var createStruct = (CREATESTRUCT) Marshal.PtrToStructure(lParam, typeof(CREATESTRUCT));
                var gcHandle = GCHandle.FromIntPtr(createStruct.lpCreateParams);
                hwndWrapper = (MessageWindow) gcHandle.Target;
                SWindowLookup.Add(hwnd, hwndWrapper);
            }
            else
            {
                if (!SWindowLookup.TryGetValue(hwnd, out hwndWrapper))
                {
                    return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
                }
            }
            Assert.IsNotNull(hwndWrapper);

            var callback = hwndWrapper._wndProcCallback;
            var ret = callback?.Invoke(hwnd, msg, wParam, lParam) ?? NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);

            if (msg != WM.NCDESTROY) return ret;
            hwndWrapper._Dispose(true);
            GC.SuppressFinalize(hwndWrapper);

            return ret;
        }

        // This isn't right if the Dispatcher has already started shutting down.
        // It will wind up leaking the class ATOM...
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        private void _Dispose(bool isHwndBeingDestroyed)
        {
            if (_isDisposed)
            {
                // Block against reentrancy.
                return;
            }

            _isDisposed = true;

            var hwnd = Handle;
            var className = _className;

            if (isHwndBeingDestroyed)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback) (arg => _DestroyWindow(IntPtr.Zero, className)));
            }
            else
                if (Handle != IntPtr.Zero)
                {
                    if (CheckAccess())
                    {
                        _DestroyWindow(hwnd, className);
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            (DispatcherOperationCallback) (arg => _DestroyWindow(hwnd, className)));
                    }
                }

            SWindowLookup.Remove(hwnd);

            _className = null;
            Handle = IntPtr.Zero;
        }
    }
}