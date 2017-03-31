using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Controls.Utilities
{
    public class MenuPopupPositionerExtension : MarkupExtension
    {
        public string ElementName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var borderClass = new BorderClass();
            borderClass.Positioner = this;

            IProvideValueTarget service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            borderClass.Border = service.TargetObject as FrameworkElement;;

            if (borderClass.Border == null)
                return this;
            var cc = new CalcClass
            {
                BorderClass = borderClass,
                Popup = borderClass.Border.FindAncestor<Popup>()
            };
            if (cc.Popup != null)
                cc.Popup.Opened += cc.Openend;
            return new Thickness(1.0);
        }

        private sealed class BorderClass
        {
            public FrameworkElement Border;
            public MenuPopupPositionerExtension Positioner;

        }

        private sealed class CalcClass
        {
            public Popup Popup;
            public BorderClass BorderClass;

            public void Openend(object sender, EventArgs e)
            {
                var name = BorderClass.Border.FindName(BorderClass.Positioner.ElementName) as FrameworkElement;
                if (name == null)
                    return;
                var screen = name.PointToScreen(new Point(0.0, 0.0));
                RECT lpRect;
                User32.GetWindowRect(((HwndSource)PresentationSource.FromVisual(Popup.Child)).Handle, out lpRect);
                if (Popup.Placement == PlacementMode.Left || Popup.Placement == PlacementMode.Right)
                {
                    BorderClass.Border.Visibility = Visibility.Collapsed;
                    if (Popup.Placement == PlacementMode.Left && lpRect.Left > screen.X)
                    {
                        Popup.HorizontalOffset = 2.0;
                    }
                    else
                    {
                        if (Popup.Placement != PlacementMode.Right || lpRect.Left >= screen.X)
                            return;
                        Popup.HorizontalOffset = -2.0;
                    }
                }
                else if (screen.Y > lpRect.Top)
                {
                    BorderClass.Border.Visibility = Visibility.Hidden;
                }
                else
                {
                    double left = 1.0 + (screen.X - lpRect.Left) * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
                    double val2 = Math.Max(0.0, (lpRect.Left + lpRect.Width - screen.X) * DpiHelper.DeviceToLogicalUnitsScalingFactorX);
                    BorderClass.Border.Margin = new Thickness(left, 0.0, 0.0, 0.0);
                    BorderClass.Border.Width = Math.Min(name.ActualWidth - 2.0, val2);
                    BorderClass.Border.Visibility = Visibility.Visible;
                }
            }
        }
}
}
