using System.Globalization;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class SearchStatusConverter : ValueConverter<int, SearchStatus>
    {
        protected override SearchStatus Convert(int value, object parameter, CultureInfo culture)
        {
            return (SearchStatus) value;
        }

        protected override int ConvertBack(SearchStatus value, object parameter, CultureInfo culture)
        {
            return (int) value;
        }
    }
}
