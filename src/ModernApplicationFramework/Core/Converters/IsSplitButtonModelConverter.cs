using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="ToBooleanValueConverter{TSource}"/> that checks if a <see cref="CommandBarDataSource"/> is a split button data model
    /// </summary>
    public class IsSplitButtonModelConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource value, object parameter, CultureInfo culture)
        {
            return value?.UiType == CommandControlTypes.SplitDropDown;
        }
    }
}
