using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Converters;
using DpiHelper = ModernApplicationFramework.Utilities.DpiHelper;

namespace ModernApplicationFramework.Basics.Imaging
{
    internal class CrispImageSourceConverter : MultiValueConverter<ImageMoniker, double, double, Color, bool, Color, bool, double, double, ObservableTask<ImageSource>>
    {
        protected override ObservableTask<ImageSource> Convert(ImageMoniker moniker, double logicalWidth, double logicalHeight, Color background, bool grayscale, Color biasColor,
            bool highContrast, double dpi, double scaleFactor, object parameter, CultureInfo culture)
        {
            try
            {
                if (ImageLibrary.Instance == null || !ImageLibrary.Instance.Initialized)
                    return null;

                return ConvertCore(moniker, logicalWidth, logicalHeight, background, grayscale, biasColor, highContrast, dpi, scaleFactor);
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static ObservableTask<ImageSource> ConvertCore(ImageMoniker moniker, double logicalWidth, double logicalHeight, Color background, bool grayscale, Color biasColor, bool highContrast, double dpi, double scaleFactor)
        {
            var num = dpi / DpiHelper.Default.LogicalDpiX * scaleFactor;
            var size = new Size
            {
                Width = logicalWidth * num,
                Height = logicalHeight * num
            };
            var attributes = new ImageAttributes
            {
                DeviceSize = new Int16Size(size),
                Background = background == Colors.Transparent ? new Color?() : background,
                Grayscale = grayscale,
                GrayscaleBiasColor = biasColor,
                HighContrast = highContrast
            };
            return ObservableTask<ImageSource>.FromResult(ImageLibrary.Instance.GetImage(moniker, attributes));
        }
    }
}
