using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class ComboBoxWidthConverter : ValueConverter<double, double>
    {
        protected override double Convert(double value, object parameter, CultureInfo culture)
        {
            return value <= 0.0 ? 90.0 : value;
        }
    }
}
