using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.EditorBase.Core.Converters
{
    internal class GridColumnWidthConverter : ValueConverter<int, double>
    {
        protected override double Convert(int value, object parameter, CultureInfo culture)
        {
            return value == 0 ? 150 : value;
        }

        protected override int ConvertBack(double value, object parameter, CultureInfo culture)
        {
            return (int) value;
        }
    }
}
