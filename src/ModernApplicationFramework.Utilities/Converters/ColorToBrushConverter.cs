using System.Globalization;
using System.Windows.Media;

namespace ModernApplicationFramework.Utilities.Converters
{
    public class ColorToBrushConverter : ValueConverter<Color, Brush>
    {
        protected override Brush Convert(Color value, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(value);
        }
    }
}
