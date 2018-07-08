using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Basics.Imaging
{
    public class MonikerSourceToImageConverter : MultiValueConverter<ImageMoniker,bool, CrispImage>
    {
        protected override CrispImage Convert(ImageMoniker moniker, bool enabled, object parameter, CultureInfo culture)
        {
            var image = new CrispImage
            {
                Moniker = moniker,
                Grayscale = enabled
            };
            return image;
        }
    }
}
