using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Native.Standard;
using Point = System.Windows.Point;
using RECT = ModernApplicationFramework.Native.Platform.Structs.RECT;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Native
{
    internal static class Screen
    {
        private static readonly List<Monitorinfo> Displays = new List<Monitorinfo>();

        public static Point ToWpf(this System.Drawing.Point p)
        {
            return new Point(p.X, p.Y);
        }

        public static Size ToWpf(this System.Drawing.Size s)
        {
            return new Size(s.Width, s.Height);
        }

        public static Rect ToWpf(this Rectangle rect)
        {
            return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
        }

        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            var matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }

        public static int DisplayCount => Displays.Count;

        internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect, out RECT monitorRect)
        {
            int displayForWindowRect = FindDisplayForWindowRect(new Rect(windowRect.Left, windowRect.Top, windowRect.Width, windowRect.Height));
            screenSubRect = new RECT
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            monitorRect = new RECT
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            if (-1 == displayForWindowRect)
                return;
            Monitorinfo display = Displays[displayForWindowRect];
            RECT rcWork = display.RcWork;
            RECT lprcDst;
            User32.IntersectRect(out lprcDst, ref rcWork, ref windowRect);
            screenSubRect = lprcDst;
            monitorRect = display.RcWork;
        }

        internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect, out Rect monitorRect)
        {
            RECT screenSubRect1;
            RECT monitorRect1;
            FindMaximumSingleMonitorRectangle(new RECT(windowRect), out screenSubRect1, out monitorRect1);
            screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
            monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
        }

        internal static void FindMonitorRectsFromPoint(Point point, out Rect monitorRect, out Rect workAreaRect)
        {
            IntPtr hMonitor = User32.MonitorFromPoint(new Platform.Structs.Point 
            {
                X = (int)point.X,
                Y = (int)point.Y
            }, 2);
            monitorRect = new Rect(0.0, 0.0, 0.0, 0.0);
            workAreaRect = new Rect(0.0, 0.0, 0.0, 0.0);
            if (!(hMonitor != IntPtr.Zero))
                return;
            var monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof(Monitorinfo))};
            User32.GetMonitorInfo(hMonitor, ref monitorInfo);
            monitorRect = new Rect(monitorInfo.RcMonitor.Position, monitorInfo.RcMonitor.Size);
            workAreaRect = new Rect(monitorInfo.RcWork.Position, monitorInfo.RcWork.Size);
        }

        public static int FindDisplayForWindowRect(Rect windowRect)
        {
            int num1 = -1;
            RECT lprcSrc2 = new RECT(windowRect);
            long num2 = 0;
            for (int index = 0; index < Displays.Count; ++index)
            {
                RECT rcWork = Displays[index].RcWork;
                RECT lprcDst;
                User32.IntersectRect(out lprcDst, ref rcWork, ref lprcSrc2);
                long num3 = lprcDst.Width * lprcDst.Height;
                if (num3 > num2)
                {
                    num1 = index;
                    num2 = num3;
                }
            }
            if (-1 == num1)
            {
                double num3 = double.MaxValue;
                for (int index = 0; index < Displays.Count; ++index)
                {
                    double num4 = Distance(Displays[index].RcMonitor, lprcSrc2);
                    if (num4 < num3)
                    {
                        num1 = index;
                        num3 = num4;
                    }
                }
            }
            return num1;
        }

        public static int FindDisplayForAbsolutePosition(Point absolutePosition)
        {
            for (int index = 0; index < Displays.Count; ++index)
            {
                RECT rcMonitor = Displays[index].RcMonitor;
                if (rcMonitor.Left <= absolutePosition.X && rcMonitor.Right >= absolutePosition.X && (rcMonitor.Top <= absolutePosition.Y && rcMonitor.Bottom >= absolutePosition.Y))
                    return index;
            }
            int num1 = -1;
            double num2 = double.MaxValue;
            for (int index = 0; index < Displays.Count; ++index)
            {
                double num3 = Distance(absolutePosition, Displays[index].RcMonitor);
                if (num3 < num2)
                {
                    num1 = index;
                    num2 = num3;
                }
            }
            return num1;
        }

        public static void AbsolutePositionToRelativePosition(double left, double top, out int display, out Point relativePosition)
        {
            display = FindDisplayForAbsolutePosition(new Point(left, top));
            relativePosition = new Point();
            if (-1 == display)
                return;
            relativePosition.X = left - Displays[display].RcMonitor.Left;
            relativePosition.Y = top - Displays[display].RcMonitor.Top;
        }

        public static void AbsoluteRectToRelativeRect(double left, double top, double width, double height, out int display, out Rect relativePosition)
        {
            AbsoluteRectToRelativeRect(new Rect(new Point(left, top), new Size(width, height)), out display, out relativePosition);
        }

        public static void AbsoluteRectToRelativeRect(Rect absoluteRect, out int display, out Rect relativeRect)
        {
            display = FindDisplayForWindowRect(absoluteRect);
            relativeRect = AbsoluteRectToRelativeRect(display, absoluteRect);
        }

        public static Rect AbsoluteRectToRelativeRect(int display, Rect absoluteRect)
        {
            Validate.IsWithinRange(display, 0, Displays.Count - 1, "display");
            RECT rcMonitor = Displays[display].RcMonitor;
            return new Rect(absoluteRect.X - rcMonitor.Left, absoluteRect.Y - rcMonitor.Top, absoluteRect.Width, absoluteRect.Height);
        }

        public static Point RelativePositionToAbsolutePosition(int display, double left, double top)
        {
            if (display < 0)
                throw new ArgumentOutOfRangeException(nameof(display));
            RECT rect;
            if (display >= Displays.Count)
            {
                Monitorinfo display1 = Displays[Displays.Count - 1];
                rect = new RECT(display1.RcMonitor.Left + display1.RcMonitor.Width, display1.RcMonitor.Top, display1.RcMonitor.Right + display1.RcMonitor.Width, display1.RcMonitor.Bottom);
            }
            else
                rect = Displays[display].RcMonitor;
            return new Point(rect.Left + left, rect.Top + top);
        }

        public static Rect RelativeRectToAbsoluteRect(int display, Rect relativeRect)
        {
            return new Rect(RelativePositionToAbsolutePosition(display, relativeRect.Left, relativeRect.Top), relativeRect.Size);
        }

        internal static Point WorkAreaToScreen(Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - monitorRect.Left + workAreaRect.Left, pt.Y - monitorRect.Top + workAreaRect.Top);
        }

        internal static Point ScreenToWorkArea(Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - workAreaRect.Left + monitorRect.Left, pt.Y - workAreaRect.Top + monitorRect.Top);
        }

        private static double Distance(RECT rect1, RECT rect2)
        {
            return Distance(GetRectCenter(rect1), GetRectCenter(rect2));
        }

        private static double Distance(Point point, RECT rect)
        {
            return Distance(point, GetRectCenter(rect));
        }

        private static double Distance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0));
        }

        private static Point GetRectCenter(RECT rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        internal static void SetDisplays(IEnumerable<Monitorinfo> displays)
        {
            Validate.IsNotNull(displays, "displays");
            Displays.Clear();
            Displays.AddRange(displays);
        }

        internal static Rect GetOnScreenPosition(Rect floatRect)
        {
            var rect1 = floatRect;
            floatRect = floatRect.LogicalToDeviceUnits();
            Rect rect2;
            Rect rect3;
            FindMaximumSingleMonitorRectangle(floatRect, out rect2, out rect3);
            if (rect2.Width == 0 || rect2.Height == 0)
            {
                FindMonitorRectsFromPoint(NativeMethods.NativeMethods.GetCursorPos(), out rect2, out rect3);
                var rect4 = rect3.DeviceToLogicalUnits();
                if (rect1.Width > rect4.Width)
                    rect1.Width = rect4.Width;
                if (rect1.Height > rect4.Height)
                    rect1.Height = rect4.Height;
                if (rect4.Right <= rect1.X)
                    rect1.X = rect4.Right - rect1.Width;
                if (rect4.Left > rect1.X + rect1.Width)
                    rect1.X = rect4.Left;
                if (rect4.Bottom <= rect1.Y)
                    rect1.Y = rect4.Bottom - rect1.Height;
                if (rect4.Top > rect1.Y + rect1.Height)
                    rect1.Y = rect4.Top;
            }
            return rect1;
        }
    }
}