using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that returns <see cref="F:System.Windows.Visibility.Visible" /> if the given <see cref="T:System.Windows.Input.ICommand" /> can be executed
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public class CommandToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iCommand = value as ICommand;
            if (iCommand != null)
            {
                return iCommand.CanExecute(parameter) ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
