using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging.Converters
{
    public class MonikerSourceToImageConverter : MultiValueConverter<Interop.ImageMoniker, bool, CrispImage>
    {
        protected override CrispImage Convert(Interop.ImageMoniker moniker, bool enabled, object parameter, CultureInfo culture)
        {
            var image = new CrispImage
            {
                Moniker = moniker,
                Grayscale = enabled
            };
            return image;
        }
    }

    public class EmptyMonikerToBoolConverter : ValueConverter<Interop.ImageMoniker, bool>
    {
        protected override bool Convert(Interop.ImageMoniker value, object parameter, CultureInfo culture)
        {
            return value.Equals(ImageLibrary.EmptyMoniker);
        }
    }
}
