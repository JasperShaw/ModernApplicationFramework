using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Platform;
using ModernApplicationFramework.Core.Utilities;

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
            if (NativeMethods.IsIconic(hwndSource.Handle))
                return base.MeasureOverride(_lastClientSize);
            NativeMethods.GetClientRect(hwndSource.Handle, out RECT lpRect);
            _lastClientSize = new Size(lpRect.Width*DpiHelper.DeviceToLogicalUnitsScalingFactorX,
                lpRect.Height*DpiHelper.DeviceToLogicalUnitsScalingFactorY);
            return base.MeasureOverride(_lastClientSize);
        }
    }
}