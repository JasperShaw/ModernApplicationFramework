using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that converts a double value to a legal combo box width. 
    /// Value zero will be returned as value 90.0
    /// </summary>
    public class ComboBoxWidthConverter : ValueConverter<double, double>
    {
        protected override double Convert(double value, object parameter, CultureInfo culture)
        {
            return value <= 0.0 ? 90.0 : value;
        }
    }
}
