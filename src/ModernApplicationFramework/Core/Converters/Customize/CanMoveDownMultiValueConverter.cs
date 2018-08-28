using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class CanMoveDownMultiValueConverter : MultiValueConverter<int, int, CommandBarDataSource, bool>
    {
        protected override bool Convert(int count, int index, CommandBarDataSource item, object parameter, CultureInfo culture)
        {
            if (index < 0 || item == null)
                return false;
            if (item.UiType == CommandControlTypes.Separator)
                return index < count - 2;
            return index < count - 1;
        }
    }
}
