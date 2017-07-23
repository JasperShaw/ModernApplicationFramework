using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:ModernApplicationFramework.Core.Converters.General.ToBooleanValueConverter`1" /> that checks whether the given <see cref="T:System.Windows.Input.ICommand" /> can be executed
    /// </summary>
    /// <seealso cref="!:ModernApplicationFramework.Core.Converters.General.ToBooleanValueConverter{System.Windows.Input.ICommand}" />
    public class CommandExecutionToBooleanConverter : ToBooleanValueConverter<ICommand>
    {
        protected override bool Convert(ICommand value, object parameter, CultureInfo culture)
        {
            return value.CanExecute(parameter);
        }
    }
}
