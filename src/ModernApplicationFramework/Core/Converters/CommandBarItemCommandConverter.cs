using System;
using System.Globalization;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that returns the embedded command of a given <see cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public class CommandBarItemCommandConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CommandBarItemDataSource definitionBase)
            {
                var c = definitionBase.CommandDefinition as CommandDefinition;
                return c?.Command;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
