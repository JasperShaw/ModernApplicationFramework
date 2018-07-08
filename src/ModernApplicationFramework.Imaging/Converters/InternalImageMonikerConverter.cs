using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging.Converters
{
    internal sealed class InternalImageMonikerConverter : ValueConverter<Interop.ImageMoniker, ImageMoniker>
    {
        internal static InternalImageMonikerConverter Instance = new InternalImageMonikerConverter();

        protected override ImageMoniker Convert(Interop.ImageMoniker moniker, object parameter, CultureInfo culture)
        {
            return moniker.ToInternalType();
        }

        protected override Interop.ImageMoniker ConvertBack(ImageMoniker moniker, object parameter, CultureInfo culture)
        {
            return moniker.ToInteropType();
        }
    }
}
