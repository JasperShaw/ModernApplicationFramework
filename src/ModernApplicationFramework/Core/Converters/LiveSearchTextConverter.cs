using System.Globalization;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    internal class LiveSearchTextConverter : MultiValueConverter<string, SearchStatus, int, string>
    {
        protected override string Convert(string value1, SearchStatus value2, int value3, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value1))
                return null;
            switch (value2)
            {
                case SearchStatus.InProgress:
                    return "Search in progress";
                case SearchStatus.Complete:
                    if (value3 == 0)
                        return "No results found";
                    if (value3 == 1)
                        return "One result found";
                    if (value3 > 1)
                        return "Many results found";
                    break;
            }

            return null;
        }
    }
}
