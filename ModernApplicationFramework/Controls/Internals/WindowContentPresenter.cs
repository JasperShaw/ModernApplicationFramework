using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class WindowContentPresenter : Decorator
    {
        private Size _lastClientSize;

        protected override Size MeasureOverride(Size constraint)
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource == null)
                return base.MeasureOverride(constraint);
            if (User32.IsIconic(hwndSource.Handle))
                return base.MeasureOverride(_lastClientSize);
            User32.GetClientRect(hwndSource.Handle, out RECT lpRect);
            _lastClientSize = new Size(lpRect.Width*DpiHelper.DeviceToLogicalUnitsScalingFactorX,
                lpRect.Height* DpiHelper.DeviceToLogicalUnitsScalingFactorY);
            return base.MeasureOverride(_lastClientSize);
        }
    }
}