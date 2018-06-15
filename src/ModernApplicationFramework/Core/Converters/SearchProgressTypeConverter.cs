using System.Globalization;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public sealed class SearchProgressTypeConverter : ToBooleanValueConverter<SearchProgressType>
    {
        protected override bool Convert(SearchProgressType value, object parameter, CultureInfo culture)
        {
            return value != SearchProgressType.Determinate;
        }
    }
}
