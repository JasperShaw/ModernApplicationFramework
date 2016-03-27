using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Core.Platform;
using Point = ModernApplicationFramework.Core.Platform.Point;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class Screen
    {
        public static System.Windows.Point ToWpf(this System.Drawing.Point p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }

        public static Size ToWpf(this System.Drawing.Size s)
        {
            return new Size(s.Width, s.Height);
        }

        public static Rect ToWpf(this System.Drawing.Rectangle rect)
        {
            return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
        }

        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }

        internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect,
            out RECT monitorRect)
        {
            var rects = new List<RECT>();
            NativeMethods.NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr lpData) =>
                {
                    Monitorinfo monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof (Monitorinfo))};
                    NativeMethods.NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
                    rects.Add(monitorInfo.RcWork);
                    return true;
                }, IntPtr.Zero);
            var num1 = 0L;
            screenSubRect = new RECT()
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            monitorRect = new RECT()
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            foreach (RECT rect in rects)
            {
                var lprcSrc1 = rect;
                RECT lprcDst;
                NativeMethods.NativeMethods.IntersectRect(out lprcDst, ref lprcSrc1, ref windowRect);
                var num2 = (long) (lprcDst.Width*lprcDst.Height);
                if (num2 > num1)
                {
                    screenSubRect = lprcDst;
                    monitorRect = rect;
                    num1 = num2;
                }
            }
        }

        internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect,
            out Rect monitorRect)
        {
            RECT screenSubRect1;
            RECT monitorRect1;
            FindMaximumSingleMonitorRectangle(new RECT(windowRect), out screenSubRect1, out monitorRect1);
            screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
            monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
        }

        internal static void FindMonitorRectsFromPoint(System.Windows.Point point, out Rect monitorRect,
            out Rect workAreaRect)
        {
            var hMonitor = NativeMethods.NativeMethods.MonitorFromPoint(new Point
            {
                X = (int) point.X,
                Y = (int) point.Y
            }, 2);
            monitorRect = new Rect(0.0, 0.0, 0.0, 0.0);
            workAreaRect = new Rect(0.0, 0.0, 0.0, 0.0);
            if (!(hMonitor != IntPtr.Zero))
                return;
            var monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof (Monitorinfo))};
            NativeMethods.NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            monitorRect = new Rect(monitorInfo.RcMonitor.Position, monitorInfo.RcMonitor.Size);
            workAreaRect = new Rect(monitorInfo.RcWork.Position, monitorInfo.RcWork.Size);
        }

        internal static System.Windows.Point ScreenToWorkArea(System.Windows.Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new System.Windows.Point(pt.X - workAreaRect.Left + monitorRect.Left,
                pt.Y - workAreaRect.Top + monitorRect.Top);
        }

        internal static System.Windows.Point WorkAreaToScreen(System.Windows.Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new System.Windows.Point(pt.X - monitorRect.Left + workAreaRect.Left,
                pt.Y - monitorRect.Top + workAreaRect.Top);
        }
    }
}