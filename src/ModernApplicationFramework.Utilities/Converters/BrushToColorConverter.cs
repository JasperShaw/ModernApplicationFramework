using System.Globalization;
using System.Windows.Media;

namespace ModernApplicationFramework.Utilities.Converters
{
    public class BrushToColorConverter : ValueConverter<Brush, Color>
    {
        protected override Color Convert(Brush value, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidColorBrush)
                return solidColorBrush.Color;
            if (value is GradientBrush gradientBrush && gradientBrush.GradientStops.Count > 0)
                return gradientBrush.GradientStops[0].Color;
            if (value == null)
                return Colors.Transparent;
            return Colors.Transparent;
        }
    }
}
