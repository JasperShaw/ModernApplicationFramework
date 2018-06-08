using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.MouseCaputreManager
{
    internal static class CaptureManager
    {
        private static bool _isSpoofingInput;

        public static void Initialize()
        {
            InputManager.Current.PostProcessInput += Current_PostProcessInput;
        }

        [DebuggerStepThrough]
        private static void Current_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            if (_isSpoofingInput)
                return;
            if (!(e.StagingItem.Input is MouseButtonEventArgs input) || input.RoutedEvent != Mouse.PreviewMouseDownOutsideCapturedElementEvent || input.Handled)
                return;
            int messagePos = User32.GetMessagePos();

            var point = new Native.Platform.Structs.Point
            {
                X = NativeMethods.GetXlParam(messagePos),
                Y = NativeMethods.GetYlParam(messagePos)
            };
            var num = User32.WindowFromPoint(point);
            if (!User32.IsWindow(num))
                return;
            if ((int)User32.GetWindowThreadProcessId(num, out var _) != (int)Kernel32.GetCurrentThreadId())
                return;
            try
            {
                _isSpoofingInput = true;
                var hwndSource = HwndSource.FromHwnd(num);
                var hitTestResult = HitTest(num);
                if (hwndSource != null && !IsHitTestResultNonClient(hitTestResult))
                    return;
                Mouse.Capture(null);
                GenerateMouseSequence(input, num, point, hitTestResult);

            }
            finally
            {
                _isSpoofingInput = false;
            }
        }

        private static void GenerateMouseSequence(MouseButtonEventArgs args, IntPtr hitWindow, Native.Platform.Structs.Point screenPoint, int hitTestResult)
        {
            var isHitTestNonClient = IsHitTestResultNonClient(hitTestResult);
            var messageFromMouseEvent = GetWindowsMessageFromMouseEvent(args, isHitTestNonClient);
            if (messageFromMouseEvent == 0)
                return;
            var ancestor = User32.GetAncestor(hitWindow, 2);
            var lParam = NativeMethods.MakeParam(hitTestResult, messageFromMouseEvent);

            bool flag1;
            bool flag2;

            switch (User32.SendMessage(hitWindow, 33, ancestor, lParam)
                .ToInt32())
            {
                case 1:
                    flag1 = flag2 = true;
                    break;
                case 2:
                    flag1 = true;
                    flag2 = false;
                    break;
                case 3:
                    flag1 = false;
                    flag2 = true;
                    break;
                default:
                    flag1 = flag2 = false;
                    break;
            }
            if (flag1)
                User32.SetActiveWindow(ancestor);
            if (!flag2)
                return;
            if (isHitTestNonClient)
                User32.SendMessage(hitWindow, messageFromMouseEvent, new IntPtr(hitTestResult), NativeMethods.MakeParam(screenPoint.X, screenPoint.Y));
            else
            {
                Native.Platform.Structs.Point point = screenPoint;
                User32.ScreenToClient(hitWindow, ref point);
                User32.SendMessage(hitWindow, messageFromMouseEvent, NativeMethods.MakeParam(PressedMouseButtons, GetXButtonWparam(args)), NativeMethods.MakeParam(point.X, point.Y));
            }

        }

        private static int PressedMouseButtons
        {
            get
            {
                int num = 0;
                if (NativeMethods.IsKeyPressed(1))
                    num |= 1;
                if (NativeMethods.IsKeyPressed(2))
                    num |= 2;
                if (NativeMethods.IsKeyPressed(4))
                    num |= 16;
                if (NativeMethods.IsKeyPressed(5))
                    num |= 32;
                if (NativeMethods.IsKeyPressed(6))
                    num |= 64;
                return num;
            }
        }

        private static int GetXButtonWparam(MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.XButton1)
                return 1;
            return args.ChangedButton == MouseButton.XButton2 ? 2 : 0;
        }

        private static bool IsHitTestResultNonClient(int hitTestResult)
        {
            return hitTestResult != 1;
        }

        private static int HitTest(IntPtr hwnd)
        {
            if (!(HwndSource.FromHwnd(hwnd)?.RootVisual is Window window))
                return 0;
            var r = window.InputHitTest(Mouse.GetPosition(window)) as UIElement;
            var p = r?.FindLogicalAncestor<INonClientArea>();
            if (p == null)
                return 1;
            return p.HitTest(Mouse.GetPosition(window));
        }

        private static int GetWindowsMessageFromMouseEvent(MouseButtonEventArgs args, bool isHitTestNonClient)
        {
            if (isHitTestNonClient)
            {
                switch (args.ChangedButton)
                {
                    case MouseButton.Left:
                        return 161;
                    case MouseButton.Middle:
                        return 167;
                    case MouseButton.Right:
                        return 164;
                    case MouseButton.XButton1:
                    case MouseButton.XButton2:
                        return 171;
                }
            }
            else
            {
                switch (args.ChangedButton)
                {
                    case MouseButton.Left:
                        return 513;
                    case MouseButton.Middle:
                        return 519;
                    case MouseButton.Right:
                        return 516;
                    case MouseButton.XButton1:
                    case MouseButton.XButton2:
                        return 523;
                }
            }
            return 0;
        }
    }
}
