﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Core.Platform
{
    public class DpiHelper
    {
        private static DpiHelper _default;
        protected const double DefaultLogicalDpi = 96.0;
        private ImageScalingMode _imageScalingMode;
        private BitmapScalingMode _bitmapScalingMode;
        private bool? _usePreScaledImages;
        private readonly MatrixTransform _transformFromDevice;
        private readonly MatrixTransform _transformToDevice;
        private double _preScaledImageLayoutTransformScaleX;
        private double _preScaledImageLayoutTransformScaleY;

        public static DpiHelper Default => _default ?? (_default = GetHelper(100));

        public ImageScalingMode ImageScalingMode
        {
            get
            {
                if (_imageScalingMode == ImageScalingMode.Default)
                {
                    int dpiScalePercentX = DpiScalePercentX;
                    ImageScalingMode imageScalingMode = GetDefaultImageScalingMode(dpiScalePercentX);
                    _imageScalingMode = GetImageScalingModeOverride(dpiScalePercentX, imageScalingMode);
                    if (!Enum.IsDefined(typeof(ImageScalingMode), _imageScalingMode) || _imageScalingMode == ImageScalingMode.Default)
                        _imageScalingMode = imageScalingMode;
                }
                return _imageScalingMode;
            }
        }

        public BitmapScalingMode BitmapScalingMode
        {
            get
            {
                if (_bitmapScalingMode == BitmapScalingMode.Unspecified)
                {
                    int dpiScalePercentX = DpiScalePercentX;
                    BitmapScalingMode bitmapScalingMode = GetDefaultBitmapScalingMode(dpiScalePercentX);
                    _bitmapScalingMode = GetBitmapScalingModeOverride(dpiScalePercentX, bitmapScalingMode);
                    if (!Enum.IsDefined(typeof(BitmapScalingMode), _bitmapScalingMode) || _bitmapScalingMode == BitmapScalingMode.Unspecified)
                        _bitmapScalingMode = bitmapScalingMode;
                }
                return _bitmapScalingMode;
            }
        }

        public bool UsePreScaledImages
        {
            get
            {
                if (!_usePreScaledImages.HasValue)
                    _usePreScaledImages = GetUsePreScaledImagesOverride(DpiScalePercentX, true);
                return _usePreScaledImages.Value;
            }
        }

        public MatrixTransform TransformFromDevice => _transformFromDevice;

        public MatrixTransform TransformToDevice => _transformToDevice;

        public double DeviceDpiX { get; }

        public double DeviceDpiY { get; }

        public double LogicalDpiX { get; }

        public double LogicalDpiY { get; }

        public bool IsScalingRequired
        {
            get
            {
                if (DeviceDpiX == LogicalDpiX)
                    return DeviceDpiY != LogicalDpiY;
                return true;
            }
        }

        public double DeviceToLogicalUnitsScalingFactorX => TransformFromDevice.Matrix.M11;

        public double DeviceToLogicalUnitsScalingFactorY => TransformFromDevice.Matrix.M22;

        public double LogicalToDeviceUnitsScalingFactorX => TransformToDevice.Matrix.M11;

        public double LogicalToDeviceUnitsScalingFactorY => TransformToDevice.Matrix.M22;

        public int DpiScalePercentX => (int)Math.Round(LogicalToDeviceUnitsScalingFactorX * 100.0);

        public int DpiScalePercentY => (int)Math.Round(LogicalToDeviceUnitsScalingFactorY * 100.0);

        public double PreScaledImageLayoutTransformScaleX
        {
            get
            {
                if (_preScaledImageLayoutTransformScaleX == 0.0)
                {
                    if (!UsePreScaledImages)
                    {
                        _preScaledImageLayoutTransformScaleX = 1.0;
                    }
                    else
                    {
                        int dpiScalePercentX = DpiScalePercentX;
                        _preScaledImageLayoutTransformScaleX = dpiScalePercentX >= 200 ? 1.0 / (dpiScalePercentX / 100) : 1.0;
                    }
                }
                return _preScaledImageLayoutTransformScaleX;
            }
        }

        public double PreScaledImageLayoutTransformScaleY
        {
            get
            {
                if (_preScaledImageLayoutTransformScaleY == 0.0)
                {
                    if (!UsePreScaledImages)
                    {
                        _preScaledImageLayoutTransformScaleY = 1.0;
                    }
                    else
                    {
                        int dpiScalePercentY = DpiScalePercentY;
                        _preScaledImageLayoutTransformScaleY = dpiScalePercentY >= 200 ? 1.0 / (dpiScalePercentY / 100) : 1.0;
                    }
                }
                return _preScaledImageLayoutTransformScaleY;
            }
        }

        protected DpiHelper(double logicalDpi)
        {
            LogicalDpiX = logicalDpi;
            LogicalDpiY = logicalDpi;
            IntPtr dc = NativeMethods.NativeMethods.GetDC(IntPtr.Zero);
            if (dc != IntPtr.Zero)
            {
                DeviceDpiX = NativeMethods.NativeMethods.GetDeviceCaps(dc, 88);
                DeviceDpiY = NativeMethods.NativeMethods.GetDeviceCaps(dc, 90);
                NativeMethods.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
            }
            else
            {
                DeviceDpiX = LogicalDpiX;
                DeviceDpiY = LogicalDpiY;
            }
            System.Windows.Media.Matrix identity1 = System.Windows.Media.Matrix.Identity;
            System.Windows.Media.Matrix identity2 = System.Windows.Media.Matrix.Identity;
            identity1.Scale(DeviceDpiX / LogicalDpiX, DeviceDpiY / LogicalDpiY);
            identity2.Scale(LogicalDpiX / DeviceDpiX, LogicalDpiY / DeviceDpiY);
            _transformFromDevice = new MatrixTransform(identity2);
            _transformFromDevice.Freeze();
            _transformToDevice = new MatrixTransform(identity1);
            _transformToDevice.Freeze();
        }

        public static DpiHelper GetHelper(int zoomPercent)
        {
            return new DpiHelper(96.0 * zoomPercent / 100.0);
        }

        private ImageScalingMode GetDefaultImageScalingMode(int dpiScalePercent)
        {
            if (dpiScalePercent % 100 == 0)
                return ImageScalingMode.NearestNeighbor;
            return dpiScalePercent < 100 ? ImageScalingMode.HighQualityBilinear : ImageScalingMode.MixedNearestNeighborHighQualityBicubic;
        }

        private BitmapScalingMode GetDefaultBitmapScalingMode(int dpiScalePercent)
        {
            if (dpiScalePercent % 100 == 0)
                return BitmapScalingMode.NearestNeighbor;
            return dpiScalePercent < 100 ? BitmapScalingMode.LowQuality : BitmapScalingMode.HighQuality;
        }

        protected virtual ImageScalingMode GetImageScalingModeOverride(int dpiScalePercent, ImageScalingMode defaultImageScalingMode)
        {
            return defaultImageScalingMode;
        }

        protected virtual BitmapScalingMode GetBitmapScalingModeOverride(int dpiScalePercent, BitmapScalingMode defaultBitmapScalingMode)
        {
            return defaultBitmapScalingMode;
        }

        protected virtual bool GetUsePreScaledImagesOverride(int dpiScalePercent, bool defaultUsePreScaledImages)
        {
            return defaultUsePreScaledImages;
        }

        private InterpolationMode GetInterpolationMode(ImageScalingMode scalingMode)
        {
            switch (scalingMode)
            {
                case ImageScalingMode.BorderOnly:
                case ImageScalingMode.NearestNeighbor:
                    return InterpolationMode.NearestNeighbor;
                case ImageScalingMode.Bilinear:
                    return InterpolationMode.Bilinear;
                case ImageScalingMode.Bicubic:
                    return InterpolationMode.Bicubic;
                case ImageScalingMode.HighQualityBilinear:
                    return InterpolationMode.HighQualityBilinear;
                case ImageScalingMode.HighQualityBicubic:
                    return InterpolationMode.HighQualityBicubic;
                default:
                    return GetInterpolationMode(ImageScalingMode);
            }
        }

        private ImageScalingMode GetActualScalingMode(ImageScalingMode scalingMode)
        {
            if (scalingMode != ImageScalingMode.Default)
                return scalingMode;
            return ImageScalingMode;
        }

        public double LogicalToDeviceUnitsX(double value)
        {
            return value * LogicalToDeviceUnitsScalingFactorX;
        }

        public double LogicalToDeviceUnitsY(double value)
        {
            return value * LogicalToDeviceUnitsScalingFactorY;
        }

        public double DeviceToLogicalUnitsX(double value)
        {
            return value * DeviceToLogicalUnitsScalingFactorX;
        }

        public double DeviceToLogicalUnitsY(double value)
        {
            return value * DeviceToLogicalUnitsScalingFactorY;
        }

        public float LogicalToDeviceUnitsX(float value)
        {
            return (float)LogicalToDeviceUnitsX((double)value);
        }

        public float LogicalToDeviceUnitsY(float value)
        {
            return (float)LogicalToDeviceUnitsY((double)value);
        }

        public int LogicalToDeviceUnitsX(int value)
        {
            return (int)Math.Round(LogicalToDeviceUnitsX((double)value));
        }

        public int LogicalToDeviceUnitsY(int value)
        {
            return (int)Math.Round(LogicalToDeviceUnitsY((double)value));
        }

        public float DeviceToLogicalUnitsX(float value)
        {
            return value * (float)DeviceToLogicalUnitsScalingFactorX;
        }

        public float DeviceToLogicalUnitsY(float value)
        {
            return value * (float)DeviceToLogicalUnitsScalingFactorY;
        }

        public int DeviceToLogicalUnitsX(int value)
        {
            return (int)Math.Round(value * DeviceToLogicalUnitsScalingFactorX);
        }

        public int DeviceToLogicalUnitsY(int value)
        {
            return (int)Math.Round(value * DeviceToLogicalUnitsScalingFactorY);
        }

        public double RoundToDeviceUnitsX(double value)
        {
            return DeviceToLogicalUnitsX(Math.Round(LogicalToDeviceUnitsX(value)));
        }

        public double RoundToDeviceUnitsY(double value)
        {
            return DeviceToLogicalUnitsY(Math.Round(LogicalToDeviceUnitsY(value)));
        }

        public System.Windows.Point LogicalToDeviceUnits(System.Windows.Point logicalPoint)
        {
            return TransformToDevice.Transform(logicalPoint);
        }

        public Rect LogicalToDeviceUnits(Rect logicalRect)
        {
            Rect rect = logicalRect;
            rect.Transform(TransformToDevice.Matrix);
            return rect;
        }

        public System.Windows.Size LogicalToDeviceUnits(System.Windows.Size logicalSize)
        {
            return new System.Windows.Size(logicalSize.Width * LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * LogicalToDeviceUnitsScalingFactorY);
        }

        public Thickness LogicalToDeviceUnits(Thickness logicalThickness)
        {
            return new Thickness(logicalThickness.Left * LogicalToDeviceUnitsScalingFactorX, logicalThickness.Top * LogicalToDeviceUnitsScalingFactorY, logicalThickness.Right * LogicalToDeviceUnitsScalingFactorX, logicalThickness.Bottom * LogicalToDeviceUnitsScalingFactorY);
        }

        public System.Windows.Point DeviceToLogicalUnits(System.Windows.Point devicePoint)
        {
            return TransformFromDevice.Transform(devicePoint);
        }

        public Rect DeviceToLogicalUnits(Rect deviceRect)
        {
            Rect rect = deviceRect;
            rect.Transform(TransformFromDevice.Matrix);
            return rect;
        }

        public System.Windows.Size DeviceToLogicalUnits(System.Windows.Size deviceSize)
        {
            return new System.Windows.Size(deviceSize.Width * DeviceToLogicalUnitsScalingFactorX, deviceSize.Height * DeviceToLogicalUnitsScalingFactorY);
        }

        public Thickness DeviceToLogicalUnits(Thickness deviceThickness)
        {
            return new Thickness(deviceThickness.Left * DeviceToLogicalUnitsScalingFactorX, deviceThickness.Top * DeviceToLogicalUnitsScalingFactorY, deviceThickness.Right * DeviceToLogicalUnitsScalingFactorX, deviceThickness.Bottom * DeviceToLogicalUnitsScalingFactorY);
        }

        public void SetDeviceLeft(ref Window window, double deviceLeft)
        {
            window.Left = deviceLeft * DeviceToLogicalUnitsScalingFactorX;
        }

        public double GetDeviceLeft(Window window)
        {
            return window.Left * LogicalToDeviceUnitsScalingFactorX;
        }

        public void SetDeviceTop(ref Window window, double deviceTop)
        {
            window.Top = deviceTop * DeviceToLogicalUnitsScalingFactorY;
        }

        public double GetDeviceTop(Window window)
        {
            return window.Top * LogicalToDeviceUnitsScalingFactorY;
        }

        public void SetDeviceWidth(ref Window window, double deviceWidth)
        {
            window.Width = deviceWidth * DeviceToLogicalUnitsScalingFactorX;
        }

        public double GetDeviceWidth(Window window)
        {
            return window.Width * LogicalToDeviceUnitsScalingFactorX;
        }

        public void SetDeviceHeight(ref Window window, double deviceHeight)
        {
            window.Height = deviceHeight * DeviceToLogicalUnitsScalingFactorY;
        }

        public double GetDeviceHeight(Window window)
        {
            return window.Height * LogicalToDeviceUnitsScalingFactorY;
        }

        public Rect GetDeviceRect(Window window)
        {
            RECT lpRect;
            NativeMethods.NativeMethods.GetWindowRect(new WindowInteropHelper(window).Handle, out lpRect);
            return new Rect(new System.Windows.Point(lpRect.Left, lpRect.Top), new System.Windows.Size(lpRect.Width, lpRect.Height));
        }

        public System.Windows.Size GetDeviceActualSize(FrameworkElement element)
        {
            return LogicalToDeviceUnits(new System.Windows.Size(element.ActualWidth, element.ActualHeight));
        }

        public System.Drawing.Point LogicalToDeviceUnits(System.Drawing.Point logicalPoint)
        {
            return new System.Drawing.Point(LogicalToDeviceUnitsX(logicalPoint.X), LogicalToDeviceUnitsY(logicalPoint.Y));
        }

        public System.Drawing.Size LogicalToDeviceUnits(System.Drawing.Size logicalSize)
        {
            return new System.Drawing.Size(LogicalToDeviceUnitsX(logicalSize.Width), LogicalToDeviceUnitsY(logicalSize.Height));
        }

        public Rectangle LogicalToDeviceUnits(Rectangle logicalRect)
        {
            return new Rectangle(LogicalToDeviceUnitsX(logicalRect.X), LogicalToDeviceUnitsY(logicalRect.Y), LogicalToDeviceUnitsX(logicalRect.Width), LogicalToDeviceUnitsY(logicalRect.Height));
        }

        public PointF LogicalToDeviceUnits(PointF logicalPoint)
        {
            return new PointF(LogicalToDeviceUnitsX(logicalPoint.X), LogicalToDeviceUnitsY(logicalPoint.Y));
        }

        public SizeF LogicalToDeviceUnits(SizeF logicalSize)
        {
            return new SizeF(LogicalToDeviceUnitsX(logicalSize.Width), LogicalToDeviceUnitsY(logicalSize.Height));
        }

        public RectangleF LogicalToDeviceUnits(RectangleF logicalRect)
        {
            return new RectangleF(LogicalToDeviceUnitsX(logicalRect.X), LogicalToDeviceUnitsY(logicalRect.Y), LogicalToDeviceUnitsX(logicalRect.Width), LogicalToDeviceUnitsY(logicalRect.Height));
        }

        public void LogicalToDeviceUnits(ref Bitmap bitmapImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            LogicalToDeviceUnits(ref bitmapImage, System.Drawing.Color.Transparent, scalingMode);
        }

        public void LogicalToDeviceUnits(ref Bitmap bitmapImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Image image = bitmapImage;
            LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
            bitmapImage = (Bitmap)image;
        }

        public void LogicalToDeviceUnits(ref Image image, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            LogicalToDeviceUnits(ref image, System.Drawing.Color.Transparent, scalingMode);
        }

        public void LogicalToDeviceUnits(ref Image image, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(image, "image");
            if (!IsScalingRequired)
                return;
            Image fromLogicalImage = CreateDeviceFromLogicalImage(image, backgroundColor, scalingMode);
            image.Dispose();
            image = fromLogicalImage;
        }

        private System.Drawing.Size GetPrescaledImageSize(System.Drawing.Size size)
        {
            return new System.Drawing.Size(size.Width * (DpiScalePercentX / 100), size.Height * (DpiScalePercentY / 100));
        }

        public Image CreateDeviceFromLogicalImage(Image logicalImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            return CreateDeviceFromLogicalImage(logicalImage, System.Drawing.Color.Transparent, scalingMode);
        }

        private void ProcessBitmapPixels(Bitmap image, PixelProcessor pixelProcessor)
        {
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bitmapdata = image.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                IntPtr scan0 = bitmapdata.Scan0;
                int length = Math.Abs(bitmapdata.Stride) * bitmapdata.Height;
                byte[] numArray = new byte[length];
                Marshal.Copy(scan0, numArray, 0, length);
                int index = 0;
                while (index < numArray.Length)
                {
                    pixelProcessor(ref numArray[index + 3], ref numArray[index + 2], ref numArray[index + 1], ref numArray[index]);
                    index += 4;
                }
                Marshal.Copy(numArray, 0, scan0, length);
            }
            finally
            {
                image.UnlockBits(bitmapdata);
            }
        }

        public ImageSource ScaleLogicalImageForDeviceSize(ImageSource image, System.Windows.Size deviceImageSize, BitmapScalingMode scalingMode)
        {
            Validate.IsNotNull(image, "image");
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(new ImageDrawing(image, new Rect(deviceImageSize)));
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                RenderOptions.SetBitmapScalingMode(drawingGroup, scalingMode);
                drawingContext.DrawDrawing(drawingGroup);
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)deviceImageSize.Width, (int)deviceImageSize.Height, LogicalDpiX, LogicalDpiY, PixelFormats.Default);
            renderTargetBitmap.Render(drawingVisual);
            BitmapFrame bitmapFrame = BitmapFrame.Create(renderTargetBitmap);
            bitmapFrame.Freeze();
            return bitmapFrame;
        }

        public Image CreateDeviceFromLogicalImage(Image logicalImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(logicalImage, "logicalImage");
            ImageScalingMode scalingMode1 = GetActualScalingMode(scalingMode);
            System.Drawing.Size size = logicalImage.Size;
            System.Drawing.Size deviceUnits = LogicalToDeviceUnits(size);
            if (scalingMode1 == ImageScalingMode.MixedNearestNeighborHighQualityBicubic)
            {
                System.Drawing.Size prescaledImageSize = GetPrescaledImageSize(size);
                if (prescaledImageSize == size)
                    scalingMode1 = ImageScalingMode.HighQualityBicubic;
                else if (prescaledImageSize == deviceUnits)
                    scalingMode1 = ImageScalingMode.NearestNeighbor;
                else if (prescaledImageSize == System.Drawing.Size.Empty)
                {
                    scalingMode1 = ImageScalingMode.HighQualityBilinear;
                }
                else
                {
                    Image image = ScaleLogicalImageForDeviceSize(logicalImage, prescaledImageSize, backgroundColor, ImageScalingMode.NearestNeighbor);
                    scalingMode1 = ImageScalingMode.HighQualityBicubic;
                    logicalImage = image;
                }
            }
            return ScaleLogicalImageForDeviceSize(logicalImage, deviceUnits, backgroundColor, scalingMode1);
        }

        private Image ScaleLogicalImageForDeviceSize(Image logicalImage, System.Drawing.Size deviceImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode)
        {
            Validate.IsNotNull(logicalImage, "logicalImage");
            InterpolationMode interpolationMode = GetInterpolationMode(scalingMode);
            System.Drawing.Imaging.PixelFormat pixelFormat = logicalImage.PixelFormat;
            System.Drawing.Color clrMagenta = System.Drawing.Color.FromArgb(byte.MaxValue, 0, byte.MaxValue);
            System.Drawing.Color clrNearGreen = System.Drawing.Color.FromArgb(0, 254, 0);
            System.Drawing.Color clrTransparentHalo = System.Drawing.Color.FromArgb(0, 246, 246, 246);
            System.Drawing.Color clrActualBackground = backgroundColor;
            Bitmap image1 = logicalImage as Bitmap;
            if (scalingMode != ImageScalingMode.NearestNeighbor && image1 != null)
            {
                if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    Rectangle rect = new Rectangle(0, 0, logicalImage.Width, logicalImage.Height);
                    logicalImage = image1.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    image1 = (Bitmap) logicalImage;
                    if (backgroundColor != System.Drawing.Color.Transparent && backgroundColor.A != byte.MaxValue)
                        backgroundColor = System.Drawing.Color.FromArgb(byte.MaxValue, backgroundColor);
                }
                ProcessBitmapPixels(image1, (ref byte alpha, ref byte red, ref byte green, ref byte blue) =>
                {
                    if (backgroundColor != System.Drawing.Color.Transparent)
                    {
                        if (alpha != backgroundColor.A || red != backgroundColor.R || (green != backgroundColor.G || blue != backgroundColor.B))
                            return;
                        alpha = clrTransparentHalo.A;
                        red = clrTransparentHalo.R;
                        green = clrTransparentHalo.G;
                        blue = clrTransparentHalo.B;
                        clrActualBackground = backgroundColor;
                    }
                    else if (alpha == clrMagenta.A && red == clrMagenta.R && (green == clrMagenta.G && blue == clrMagenta.B))
                    {
                        alpha = clrTransparentHalo.A;
                        red = clrTransparentHalo.R;
                        green = clrTransparentHalo.G;
                        blue = clrTransparentHalo.B;
                        clrActualBackground = clrMagenta;
                    }
                    else
                    {
                        if (alpha != clrNearGreen.A || red != clrNearGreen.R || (green != clrNearGreen.G || blue != clrNearGreen.B))
                            return;
                        alpha = clrTransparentHalo.A;
                        red = clrTransparentHalo.R;
                        green = clrTransparentHalo.G;
                        blue = clrTransparentHalo.B;
                        clrActualBackground = clrNearGreen;
                    }
                });
                if (clrActualBackground == System.Drawing.Color.Transparent && pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    clrActualBackground = !(backgroundColor != System.Drawing.Color.Transparent) ? clrMagenta : backgroundColor;
            }
            Image image2;
            if (logicalImage is Bitmap)
            {
                image2 = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);
            }
            else
            {
                if (!(logicalImage is Metafile))
                    throw new ArgumentException("Unsupported image type for High DPI conversion", nameof(logicalImage));
                IntPtr dc = NativeMethods.NativeMethods.GetDC(IntPtr.Zero);
                try
                {
                    image2 = new Metafile(dc, EmfType.EmfPlusDual);
                }
                finally
                {
                    NativeMethods.NativeMethods.ReleaseDC(IntPtr.Zero, dc);
                }
            }
            using (Graphics graphics = Graphics.FromImage(image2))
            {
                graphics.InterpolationMode = interpolationMode;
                graphics.Clear(backgroundColor);
                RectangleF srcRect = new RectangleF(0.0f, 0.0f, logicalImage.Size.Width, logicalImage.Size.Height);
                srcRect.Offset(-0.5f, -0.5f);
                RectangleF destRect = new RectangleF(0.0f, 0.0f, deviceImageSize.Width, deviceImageSize.Height);
                if (scalingMode == ImageScalingMode.BorderOnly)
                {
                    destRect = new RectangleF(0.0f, 0.0f, srcRect.Width, srcRect.Height);
                    destRect.Offset((float)((deviceImageSize.Width - (double)srcRect.Width) / 2.0), (float)((deviceImageSize.Height - (double)srcRect.Height) / 2.0));
                }
                graphics.DrawImage(logicalImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
            Bitmap image3 = image2 as Bitmap;
            if (scalingMode != ImageScalingMode.NearestNeighbor && image3 != null)
            {
                ProcessBitmapPixels(image3, (ref byte alpha, ref byte red, ref byte green, ref byte blue) =>
                {
                    if (alpha == byte.MaxValue)
                        return;
                    alpha = clrActualBackground.A;
                    red = clrActualBackground.R;
                    green = clrActualBackground.G;
                    blue = clrActualBackground.B;
                });
                if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    Rectangle rect = new Rectangle(0, 0, image2.Width, image2.Height);
                    image2 = image3.Clone(rect, pixelFormat);
                }
            }
            return image2;
        }

        public void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            LogicalToDeviceUnits(ref imageStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
        }

        public void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(imageStrip, "imageStrip");
            if (!IsScalingRequired)
                return;
            Bitmap fromLogicalImage = CreateDeviceFromLogicalImage(imageStrip, logicalImageSize, backgroundColor, scalingMode);
            imageStrip.Dispose();
            imageStrip = fromLogicalImage;
        }

        public Bitmap CreateDeviceFromLogicalImage(Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            return CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
        }

        public Bitmap CreateDeviceFromLogicalImage(Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(logicalBitmapStrip, "logicalBitmapStrip");
            Validate.IsNotNull(logicalImageSize, "logicalImageSize");
            if (logicalImageSize.Width == 0 || logicalBitmapStrip.Height % logicalImageSize.Width != 0 || logicalImageSize.Height != logicalBitmapStrip.Height)
                throw new ArgumentException("logicalImageSize not matching the logicalBitmap size");
            int num = logicalBitmapStrip.Width / logicalImageSize.Width;
            int deviceUnitsX = LogicalToDeviceUnitsX(logicalImageSize.Width);
            int deviceUnitsY = LogicalToDeviceUnitsY(logicalImageSize.Height);
            Bitmap bitmap = new Bitmap(num * deviceUnitsX, deviceUnitsY, logicalBitmapStrip.PixelFormat);
            using (Graphics graphics1 = Graphics.FromImage(bitmap))
            {
                graphics1.InterpolationMode = InterpolationMode.NearestNeighbor;
                for (int index = 0; index < num; ++index)
                {
                    RectangleF srcRect = new RectangleF(index * logicalImageSize.Width, 0.0f, logicalImageSize.Width, logicalImageSize.Height);
                    srcRect.Offset(-0.5f, -0.5f);
                    RectangleF destRect = new RectangleF(0.0f, 0.0f, logicalImageSize.Width, logicalImageSize.Height);
                    Bitmap bitmapImage = new Bitmap(logicalImageSize.Width, logicalImageSize.Height, logicalBitmapStrip.PixelFormat);
                    using (Graphics graphics2 = Graphics.FromImage(bitmapImage))
                    {
                        graphics2.InterpolationMode = InterpolationMode.NearestNeighbor;
                        graphics2.DrawImage(logicalBitmapStrip, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    LogicalToDeviceUnits(ref bitmapImage, backgroundColor, scalingMode);
                    srcRect = new RectangleF(0.0f, 0.0f, deviceUnitsX, deviceUnitsY);
                    srcRect.Offset(-0.5f, -0.5f);
                    destRect = new RectangleF(index * deviceUnitsX, 0.0f, deviceUnitsX, deviceUnitsY);
                    graphics1.DrawImage(bitmapImage, destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
            return bitmap;
        }

        public void LogicalToDeviceUnits(ref Icon icon, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(icon, "icon");
            if (!IsScalingRequired)
                return;
            Icon fromLogicalImage = CreateDeviceFromLogicalImage(icon, scalingMode);
            icon.Dispose();
            icon = fromLogicalImage;
        }

        public Icon CreateDeviceFromLogicalImage(Icon logicalIcon, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(logicalIcon, "logicalIcon");
            System.Drawing.Size deviceUnits = LogicalToDeviceUnits(logicalIcon.Size);
            Icon icon = new Icon(logicalIcon, deviceUnits);
            if (icon.Size.Width != deviceUnits.Width && icon.Size.Width != 0)
            {
                IntPtr hicon = ((Bitmap)CreateDeviceFromLogicalImage(icon.ToBitmap(), System.Drawing.Color.Transparent, scalingMode)).GetHicon();
                icon = Icon.FromHandle(hicon).Clone() as Icon;
                NativeMethods.NativeMethods.DestroyIcon(hicon);
            }
            return icon;
        }

        public void LogicalToDeviceUnits(ref ImageList imageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            LogicalToDeviceUnits(ref imageList, System.Drawing.Color.Transparent, scalingMode);
        }

        public void LogicalToDeviceUnits(ref ImageList imageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(imageList, "imageList");
            if (!IsScalingRequired)
                return;
            ImageList fromLogicalImage = CreateDeviceFromLogicalImage(imageList, backgroundColor, scalingMode);
            imageList.Dispose();
            imageList = fromLogicalImage;
        }

        public ImageList CreateDeviceFromLogicalImage(ImageList logicalImageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            return CreateDeviceFromLogicalImage(logicalImageList, System.Drawing.Color.Transparent, scalingMode);
        }

        public ImageList CreateDeviceFromLogicalImage(ImageList logicalImageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
        {
            Validate.IsNotNull(logicalImageList, "logicalImageList");
            ImageList imageList = new ImageList
            {
                Site = logicalImageList.Site,
                Tag = logicalImageList.Tag,
                ColorDepth = logicalImageList.ColorDepth,
                TransparentColor = logicalImageList.TransparentColor,
                ImageSize = LogicalToDeviceUnits(logicalImageList.ImageSize)
            };
            for (int index = 0; index < logicalImageList.Images.Count; ++index)
            {
                Image fromLogicalImage = CreateDeviceFromLogicalImage(logicalImageList.Images[index], backgroundColor, scalingMode);
                imageList.Images.Add(fromLogicalImage);
            }
            foreach (string key in logicalImageList.Images.Keys)
            {
                int index = logicalImageList.Images.IndexOfKey(key);
                if (index != -1)
                    imageList.Images.SetKeyName(index, key);
            }
            return imageList;
        }

        private delegate void PixelProcessor(ref byte alpha, ref byte red, ref byte green, ref byte blue);
    }
}