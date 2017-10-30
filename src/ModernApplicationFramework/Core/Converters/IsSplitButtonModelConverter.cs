using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="ToBooleanValueConverter{TSource}"/> that checks if a <see cref="CommandBarDefinitionBase"/> is a split button data model
    /// </summary>
    public class IsSplitButtonModelConverter : ToBooleanValueConverter<CommandBarDefinitionBase>
    {
        protected override bool Convert(CommandBarDefinitionBase value, object parameter, CultureInfo culture)
        {
            return value?.CommandDefinition.ControlType == CommandControlTypes.SplitDropDown;
        }
    }
}
