using System.Globalization;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Imaging
{
    public sealed class ActualDpiConverter : ValueConverter<double, double>
    {
        public static readonly ActualDpiConverter Instance = new ActualDpiConverter();

        protected override double Convert(double dpi, object parameter, CultureInfo culture)
        {
            return dpi.AreClose(0.0) ? CrispImage.DefaultDpi : dpi;
        }
    }
}
