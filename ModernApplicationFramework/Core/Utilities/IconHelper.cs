using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class IconHelper
    {
        private static bool _triedGettingIconsFromExecutable;
        private static bool _isWindowIconRetrieved;
        private static readonly object SyncLock = new object();
        private static BitmapSource _smallIcon;
        private static BitmapSource _largeIcon;
        private static BitmapSource _windowIcon;

        public static async void UseWindowIconAsync(Action<ImageSource> callback)
        {
            if (_isWindowIconRetrieved)
            {
                callback(_windowIcon);
            }
            else
            {
                if (_windowIcon == null)
                {
                    if (!_triedGettingIconsFromExecutable)
                        await Task.Run(() =>
                        {
                            GetWindowIcon(() => ExtractIconsFromExecutable(ref _smallIcon, ref _largeIcon), ref _triedGettingIconsFromExecutable);
                        });
                    GetWindowIcon(() => ChooseOrEncodeWindowIcon(_smallIcon, _largeIcon), ref _isWindowIconRetrieved);
                }
                callback(_windowIcon);
            }
        }

        private static void GetWindowIcon(Func<BitmapSource> imageGetter, ref bool imageGotFlag)
        {
            lock (SyncLock)
            {
                if (imageGotFlag)
                    return;
                try
                {
                    _windowIcon = imageGetter();
                }
                finally
                {
                    imageGotFlag = true;
                }
            }
        }

        private static BitmapSource ExtractIconsFromExecutable(ref BitmapSource smallIcon, ref BitmapSource largeIcon)
        {
            var executablePath = System.Windows.Forms.Application.ExecutablePath;
            var phiconLarge = new[]
            {
                IntPtr.Zero
            };
            var phiconSmall = new[]
            {
                IntPtr.Zero
            };
            if (Shell32.ExtractIconEx(executablePath.Trim('"'), 0, phiconLarge, phiconSmall, 1) <= 0)
                return null;
            smallIcon = BitmapSourceFromHIcon(phiconSmall[0]);
            largeIcon = BitmapSourceFromHIcon(phiconLarge[0]);
            return null;
        }

        private static BitmapSource BitmapSourceFromHIcon(IntPtr iconHandle)
        {
            BitmapSource bitmapSource = null;
            if (iconHandle != IntPtr.Zero)
            {
                bitmapSource = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                User32.DestroyIcon(iconHandle);
                FreezeImage(bitmapSource);
            }
            return bitmapSource;
        }

        private static void FreezeImage(Freezable image)
        {
            if (image == null || image.IsFrozen || !image.CanFreeze)
                return;
            image.Freeze();
        }

        private static BitmapSource ChooseOrEncodeWindowIcon(BitmapSource smallIcon, BitmapSource largeIcon)
        {
            BitmapSource bitmapSource = null;
            if (largeIcon != null)
            {
                if (smallIcon != null)
                {
                    BitmapFrame bitmapFrame;
                    var tiffBitmapEncoder = new TiffBitmapEncoder();
                    tiffBitmapEncoder.Frames.Add(BitmapFrame.Create(smallIcon));
                    tiffBitmapEncoder.Frames.Add(BitmapFrame.Create(largeIcon));
                    using (var memoryStream = new MemoryStream())
                    {
                        tiffBitmapEncoder.Save(memoryStream);
                        bitmapFrame = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    FreezeImage(bitmapFrame);
                    bitmapSource = bitmapFrame;
                }
                else
                    bitmapSource = largeIcon;
            }
            else if (smallIcon != null)
                bitmapSource = smallIcon;
            return bitmapSource;
        }
    }
}