using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Platform.Structs;

namespace ModernApplicationFramework.Controls
{
    public class WindowContentPresenter : Decorator
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
            _lastClientSize = new Size(lpRect.Width*Core.Utilities.DpiHelper.DeviceToLogicalUnitsScalingFactorX,
                lpRect.Height* Core.Utilities.DpiHelper.DeviceToLogicalUnitsScalingFactorY);
            return base.MeasureOverride(_lastClientSize);
        }
    }
}