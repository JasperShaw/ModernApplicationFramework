using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class DpiHelper
    {
        // ReSharper disable once InconsistentNaming
        private static readonly MatrixTransform transformFromDevice;
        // ReSharper disable once InconsistentNaming
        private static readonly MatrixTransform transformToDevice;

        public static double DeviceDpiX { get; private set; }

        public static double DeviceDpiY { get; private set; }

        public static MatrixTransform TransformFromDevice
        {
            get
            {
                return transformFromDevice;
            }
        }

        public static double DeviceToLogicalUnitsScalingFactorX
        {
            get
            {
                return TransformFromDevice.Matrix.M11;
            }
        }

        public static double DeviceToLogicalUnitsScalingFactorY
        {
            get
            {
                return TransformFromDevice.Matrix.M22;
            }
        }

        public static MatrixTransform TransformToDevice
        {
            get
            {
                return transformToDevice;
            }
        }

        public static double LogicalToDeviceUnitsScalingFactorX
        {
            get
            {
                return TransformToDevice.Matrix.M11;
            }
        }

        public static double LogicalToDeviceUnitsScalingFactorY
        {
            get
            {
                return TransformToDevice.Matrix.M22;
            }
        }

        static DpiHelper()
        {
            var dc = NativeMethods.NativeMethods.GetDC(IntPtr.Zero);
            if (dc != IntPtr.Zero)
            {
                DeviceDpiX = NativeMethods.NativeMethods.GetDeviceCaps(dc, 88);
                DeviceDpiY = NativeMethods.NativeMethods.GetDeviceCaps(dc, 90);
                NativeMethods.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
            }
            else
            {
                DeviceDpiX = 96.0;
                DeviceDpiY = 96.0;
            }
            var identity1 = Matrix.Identity;
            var identity2 = Matrix.Identity;
            identity1.Scale(DeviceDpiX / 96.0, DeviceDpiY / 96.0);
            identity2.Scale(96.0 / DeviceDpiX, 96.0 / DeviceDpiY);
            transformFromDevice = new MatrixTransform(identity2);
            transformFromDevice.Freeze();
            transformToDevice = new MatrixTransform(identity1);
            transformToDevice.Freeze();
        }

        public static Rect LogicalToDeviceUnits(this Rect logicalRect)
        {
            var rect = logicalRect;
            rect.Transform(TransformToDevice.Matrix);
            return rect;
        }

        public static Size LogicalToDeviceUnits(this Size logicalSize)
        {
            return new Size(logicalSize.Width * LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * LogicalToDeviceUnitsScalingFactorY);
        }

        public static Rect DeviceToLogicalUnits(this Rect deviceRect)
        {
            Rect rect = deviceRect;
            rect.Transform(TransformFromDevice.Matrix);
            return rect;
        }
    }
}
