using System.Globalization;
using System.Windows.Media;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class BrushToColorConverter : ValueConverter<Brush, Color>
    {
        protected override Color Convert(Brush value, object parameter, CultureInfo culture)
        {
            SolidColorBrush solidColorBrush = value as SolidColorBrush;
            if (solidColorBrush != null)
                return solidColorBrush.Color;
            GradientBrush gradientBrush = value as GradientBrush;
            if (gradientBrush != null && gradientBrush.GradientStops.Count > 0)
                return gradientBrush.GradientStops[0].Color;
            if (value == null)
                return Colors.Transparent;
            return Colors.Transparent;
        }
    }
}
