using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Core.Platform;
using ModernApplicationFramework.Core.Standard;
using Point = System.Windows.Point;
using RECT = ModernApplicationFramework.Core.Platform.RECT;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class Screen
    {
        private static List<Monitorinfo> _displays = new List<Monitorinfo>();

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

        public static int DisplayCount
        {
            get
            {
                return Screen._displays.Count;
            }
        }

        internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect, out RECT monitorRect)
        {
            int displayForWindowRect = Screen.FindDisplayForWindowRect(new Rect((double)windowRect.Left, (double)windowRect.Top, (double)windowRect.Width, (double)windowRect.Height));
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
            if (-1 == displayForWindowRect)
                return;
            Monitorinfo display = Screen._displays[displayForWindowRect];
            RECT rcWork = display.RcWork;
            RECT lprcDst;
            NativeMethods.NativeMethods.IntersectRect(out lprcDst, ref rcWork, ref windowRect);
            screenSubRect = lprcDst;
            monitorRect = display.RcWork;
        }

        internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect, out Rect monitorRect)
        {
            RECT screenSubRect1;
            RECT monitorRect1;
            Screen.FindMaximumSingleMonitorRectangle(new RECT(windowRect), out screenSubRect1, out monitorRect1);
            screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
            monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
        }

        internal static void FindMonitorRectsFromPoint(Point point, out Rect monitorRect, out Rect workAreaRect)
        {
            IntPtr hMonitor = NativeMethods.NativeMethods.MonitorFromPoint(new Platform.Point 
            {
                X = (int)point.X,
                Y = (int)point.Y
            }, 2);
            monitorRect = new Rect(0.0, 0.0, 0.0, 0.0);
            workAreaRect = new Rect(0.0, 0.0, 0.0, 0.0);
            if (!(hMonitor != IntPtr.Zero))
                return;
            var monitorInfo = new Monitorinfo();
            monitorInfo.CbSize = (uint)Marshal.SizeOf(typeof(Monitorinfo));
            NativeMethods.NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            monitorRect = new Rect(monitorInfo.RcMonitor.Position, monitorInfo.RcMonitor.Size);
            workAreaRect = new Rect(monitorInfo.RcWork.Position, monitorInfo.RcWork.Size);
        }

        public static int FindDisplayForWindowRect(Rect windowRect)
        {
            int num1 = -1;
            RECT lprcSrc2 = new RECT(windowRect);
            long num2 = 0;
            for (int index = 0; index < Screen._displays.Count; ++index)
            {
                RECT rcWork = Screen._displays[index].RcWork;
                RECT lprcDst;
                NativeMethods.NativeMethods.IntersectRect(out lprcDst, ref rcWork, ref lprcSrc2);
                long num3 = (long)(lprcDst.Width * lprcDst.Height);
                if (num3 > num2)
                {
                    num1 = index;
                    num2 = num3;
                }
            }
            if (-1 == num1)
            {
                double num3 = double.MaxValue;
                for (int index = 0; index < Screen._displays.Count; ++index)
                {
                    double num4 = Screen.Distance(Screen._displays[index].RcMonitor, lprcSrc2);
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
            for (int index = 0; index < Screen._displays.Count; ++index)
            {
                RECT rcMonitor = Screen._displays[index].RcMonitor;
                if ((double)rcMonitor.Left <= absolutePosition.X && (double)rcMonitor.Right >= absolutePosition.X && ((double)rcMonitor.Top <= absolutePosition.Y && (double)rcMonitor.Bottom >= absolutePosition.Y))
                    return index;
            }
            int num1 = -1;
            double num2 = double.MaxValue;
            for (int index = 0; index < Screen._displays.Count; ++index)
            {
                double num3 = Screen.Distance(absolutePosition, Screen._displays[index].RcMonitor);
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
            display = Screen.FindDisplayForAbsolutePosition(new Point(left, top));
            relativePosition = new Point();
            if (-1 == display)
                return;
            relativePosition.X = left - (double)Screen._displays[display].RcMonitor.Left;
            relativePosition.Y = top - (double)Screen._displays[display].RcMonitor.Top;
        }

        public static void AbsoluteRectToRelativeRect(double left, double top, double width, double height, out int display, out Rect relativePosition)
        {
            Screen.AbsoluteRectToRelativeRect(new Rect(new Point(left, top), new Size(width, height)), out display, out relativePosition);
        }

        public static void AbsoluteRectToRelativeRect(Rect absoluteRect, out int display, out Rect relativeRect)
        {
            display = Screen.FindDisplayForWindowRect(absoluteRect);
            relativeRect = Screen.AbsoluteRectToRelativeRect(display, absoluteRect);
        }

        public static Rect AbsoluteRectToRelativeRect(int display, Rect absoluteRect)
        {
            Validate.IsWithinRange(display, 0, Screen._displays.Count - 1, "display");
            RECT rcMonitor = Screen._displays[display].RcMonitor;
            return new Rect(absoluteRect.X - (double)rcMonitor.Left, absoluteRect.Y - (double)rcMonitor.Top, absoluteRect.Width, absoluteRect.Height);
        }

        public static Point RelativePositionToAbsolutePosition(int display, double left, double top)
        {
            if (display < 0)
                throw new ArgumentOutOfRangeException("display");
            RECT rect;
            if (display >= Screen._displays.Count)
            {
                Monitorinfo display1 = Screen._displays[Screen._displays.Count - 1];
                rect = new RECT(display1.RcMonitor.Left + display1.RcMonitor.Width, display1.RcMonitor.Top, display1.RcMonitor.Right + display1.RcMonitor.Width, display1.RcMonitor.Bottom);
            }
            else
                rect = Screen._displays[display].RcMonitor;
            return new Point((double)rect.Left + left, (double)rect.Top + top);
        }

        public static Rect RelativeRectToAbsoluteRect(int display, Rect relativeRect)
        {
            return new Rect(Screen.RelativePositionToAbsolutePosition(display, relativeRect.Left, relativeRect.Top), relativeRect.Size);
        }

        internal static Point WorkAreaToScreen(Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            Screen.FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - monitorRect.Left + workAreaRect.Left, pt.Y - monitorRect.Top + workAreaRect.Top);
        }

        internal static Point ScreenToWorkArea(Point pt)
        {
            Rect monitorRect;
            Rect workAreaRect;
            Screen.FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - workAreaRect.Left + monitorRect.Left, pt.Y - workAreaRect.Top + monitorRect.Top);
        }

        private static double Distance(RECT rect1, RECT rect2)
        {
            return Screen.Distance(Screen.GetRectCenter(rect1), Screen.GetRectCenter(rect2));
        }

        private static double Distance(Point point, RECT rect)
        {
            return Screen.Distance(point, Screen.GetRectCenter(rect));
        }

        private static double Distance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0));
        }

        private static Point GetRectCenter(RECT rect)
        {
            return new Point((double)(rect.Left + rect.Width / 2), (double)(rect.Top + rect.Height / 2));
        }

        internal static void SetDisplays(IEnumerable<Monitorinfo> displays)
        {
            Validate.IsNotNull((object)displays, "displays");
            Screen._displays.Clear();
            Screen._displays.AddRange(displays);
        }
    }
}