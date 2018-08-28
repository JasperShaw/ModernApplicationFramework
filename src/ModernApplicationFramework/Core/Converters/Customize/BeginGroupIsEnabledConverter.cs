using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class BeginGroupIsEnabledConverter : ToBooleanValueConverter<CommandBarDataSource>
    {
        protected override bool Convert(CommandBarDataSource value, object parameter, CultureInfo culture)
        {
            if (value is CommandBarItemDataSource itemDefinition)
                return !itemDefinition.IsVeryFirst;
            return false;
        }
    }
}
