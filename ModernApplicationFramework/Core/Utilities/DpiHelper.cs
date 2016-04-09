using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class DpiHelper
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once InconsistentNaming

        static DpiHelper()
        {
            var x = GetScalingFactor();

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
        }

        public static double DeviceDpiX { get; }

        public static double DeviceDpiY { get; }

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

        public static double GetScalingFactor()
        {
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);

            var dpiX = 96.0;
            if (source?.CompositionTarget != null)
                dpiX = 96.0*source.CompositionTarget.TransformToDevice.M11;
            return dpiX;
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