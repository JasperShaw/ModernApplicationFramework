using System;

namespace ModernApplicationFramework.Docking.NativeMethods
{
    internal static class NativeMethods
    {
        internal struct Point
        {
            public int X;
            public int Y;
        }

        internal enum GetWindowCmd : uint
        {
            GwHwndfirst = 0,
            GwHwndlast = 1,
            GwHwndnext = 2,
            GwHwndprev = 3,
            GwOwner = 4,
            GwChild = 5,
            GwEnabledpopup = 6
        }

        internal static bool IsLeftButtonPressed()
        {
            return IsKeyPressed(1);
        }

        internal static IntPtr SetActiveWindow(IntPtr hwnd)
        {
            var ret = User32.SetActiveWindow(hwnd);
            if (ret == IntPtr.Zero)
                Hresult.ThrowLastError();
            return ret;
        }

        internal static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
        {
            User32.SetWindowLong(
                childHandle,
                -8,
                ownerHandle.ToInt32());
        }

        internal static System.Windows.Point GetMousePosition()
        {
            var w32Mouse = new Point();
            User32.GetCursorPos(ref w32Mouse);
            return new System.Windows.Point(w32Mouse.X, w32Mouse.Y);
        }

        internal static bool IsControlPressed()
        {
            return IsKeyPressed(17);
        }

        internal static bool IsKeyPressed(int vKey)
        {
            return User32.GetKeyState(vKey) < 0;
        }

        internal static int HiWord(int value)
        {
            return (short)(value >> 16);
        }

        internal static int LoWord(int value)
        {
            return (short)(value & ushort.MaxValue);
        }

        internal static IntPtr MakeParam(int lowWord, int highWord)
        {
            return new IntPtr((lowWord & ushort.MaxValue) | (highWord << 16));
        }

        internal static RECT GetClientRect(IntPtr hWnd)
        {
            User32.GetClientRect(hWnd, out var result);
            return result;
        }

        internal static RECT GetWindowRect(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out var result);
            return result;
        }

        internal static System.Windows.Point GetCursorPos()
        {
            var point1 = new Point { X = 0, Y = 0 };
            var point2 = new System.Windows.Point();
            if (User32.GetCursorPos(ref point1))
            {
                point2.X = point1.X;
                point2.Y = point1.Y;
            }
            return point2;
        }

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

    }
}
