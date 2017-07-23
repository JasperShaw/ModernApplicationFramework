using System;
using System.Globalization;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IValueConverter" /> that checks whether a <see cref="CommandBarDefinitionBase" /> is <see cref="P:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase.IsCustom" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class IsCustomMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CommandBarDefinitionBase definition)
                return definition.IsCustom;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
