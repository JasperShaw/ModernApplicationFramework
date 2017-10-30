using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <summary>
    /// A <see cref="MultiValueConverter{TSource1,TSource2,TTarget}"/> that checks if two input parameters are equal
    /// </summary>
    public class AreEqualConverter : MultiValueConverter<object, object, bool>
    {
        protected override bool Convert(object value1, object value2, object parameter, CultureInfo culture)
        {
            if (Equals(value1, value2))
                return true;
            return value1 != null && value1.Equals(value2);
        }
    }
}
