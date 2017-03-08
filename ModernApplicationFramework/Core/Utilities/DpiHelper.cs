using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class DpiHelper
    {
        static DpiHelper()
        {
            var x = GetApplicationsScalingFactor();

            var dpiX = 96.0*x/100;
            var dpiY = 96.0*x/100;


            var dc = NativeMethods.NativeMethods.GetDC(IntPtr.Zero);
            if (dc != IntPtr.Zero)
            {
                DeviceDpiX = NativeMethods.NativeMethods.GetDeviceCaps(dc, 88);
                DeviceDpiY = NativeMethods.NativeMethods.GetDeviceCaps(dc, 90);
                NativeMethods.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
            }
            else
            {
                DeviceDpiX = dpiX;
                DeviceDpiY = dpiY;
            }
            var identity1 = Matrix.Identity;
            var identity2 = Matrix.Identity;
            identity1.Scale(DeviceDpiX/dpiX, DeviceDpiY/dpiY);
            identity2.Scale(dpiX/DeviceDpiX, dpiY/DeviceDpiY);
            TransformFromDevice = new MatrixTransform(identity2);
            TransformFromDevice.Freeze();
            TransformToDevice = new MatrixTransform(identity1);
            TransformToDevice.Freeze();

            IsNormallyScaled = GetScalingFactor();
        }

        public static double DeviceDpiX { get; }

        public static double DeviceDpiY { get; }

        public static bool IsNormallyScaled { get; }

        public static double DeviceToLogicalUnitsScalingFactorX => TransformFromDevice.Matrix.M11;

        public static double DeviceToLogicalUnitsScalingFactorY => TransformFromDevice.Matrix.M22;

        public static double LogicalToDeviceUnitsScalingFactorX => TransformToDevice.Matrix.M11;

        public static double LogicalToDeviceUnitsScalingFactorY => TransformToDevice.Matrix.M22;

        public static MatrixTransform TransformFromDevice { get; }

        public static MatrixTransform TransformToDevice { get; }

        public static Rect DeviceToLogicalUnits(this Rect deviceRect)
        {
            var rect = deviceRect;
            rect.Transform(TransformFromDevice.Matrix);
            return rect;
        }

        public static double GetApplicationsScalingFactor()
        {
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);

            var dpiX = 96.0;
            if (source?.CompositionTarget != null)
                dpiX = 96.0*source.CompositionTarget.TransformToDevice.M11;
            return dpiX;
        }


        private static bool GetScalingFactor()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            var desktop = g.GetHdc();
            var logicalScreenHeight = NativeMethods.NativeMethods.GetDeviceCaps(desktop, (int)NativeMethods.NativeMethods.DeviceCap.Vertres);
            var physicalScreenHeight = NativeMethods.NativeMethods.GetDeviceCaps(desktop, (int)NativeMethods.NativeMethods.DeviceCap.Desktopvertres);
            var logpixelsy = NativeMethods.NativeMethods.GetDeviceCaps(desktop, (int)NativeMethods.NativeMethods.DeviceCap.Logpixelsy);
            var screenScalingFactor = physicalScreenHeight / (float)logicalScreenHeight;
            var dpiScalingFactor = logpixelsy / (float)96;

            return !(screenScalingFactor > 1) && !(dpiScalingFactor > 1);
        }

        public static Rect LogicalToDeviceUnits(this Rect logicalRect)
        {
            var rect = logicalRect;
            rect.Transform(TransformToDevice.Matrix);
            return rect;
        }

        public static Size LogicalToDeviceUnits(this Size logicalSize)
        {
            return new Size(logicalSize.Width*LogicalToDeviceUnitsScalingFactorX,
                logicalSize.Height*LogicalToDeviceUnitsScalingFactorY);
        }
    }
}