using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Basics.Imaging
{
    public class MonikerSourceToImageConverter : ValueConverter<ImageMoniker, CrispImage>
    {
        protected override CrispImage Convert(ImageMoniker value1, object parameter, CultureInfo culture)
        {
            var image = new CrispImage();
            image.Moniker = value1;
            return image;
        }
    }
}
