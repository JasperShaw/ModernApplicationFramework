using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class CanMoveUpMultiValueConverter : MultiValueConverter<int, CommandBarDataSource, bool>
    {
        protected override bool Convert(int index, CommandBarDataSource item, object parameter, CultureInfo culture)
        {
            if (index < 0 || item == null)
                return false;
            if (item.UiType == CommandControlTypes.Separator)
                return index > 1;
            return index > 0;
        }
    }
}
