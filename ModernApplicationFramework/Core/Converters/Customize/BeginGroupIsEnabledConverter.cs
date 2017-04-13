using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class BeginGroupIsEnabledConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
                return value?.SortOrder > 0;
        }
    }
}
