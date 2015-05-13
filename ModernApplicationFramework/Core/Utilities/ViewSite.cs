using System.Windows;

namespace ModernApplicationFramework.Core.Utilities
{
    public abstract class ViewSite
    {
        internal static Rect GetOnScreenPosition(Rect floatRect)
        {
            Rect rect1 = floatRect;
            floatRect = floatRect.LogicalToDeviceUnits();
            Rect rect2;
            Rect rect3;
            Screen.FindMaximumSingleMonitorRectangle(floatRect, out rect2, out rect3);
            if (floatRect.IntersectsWith(rect3))
                return rect1;
            Screen.FindMonitorRectsFromPoint(NativeMethods.NativeMethods.GetCursorPos(), out rect2, out rect3);
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
            return rect1;
        }
    }
}
