using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public class CommandExecutionToBooleanConverter : ToBooleanValueConverter<ICommand>
    {
        protected override bool Convert(ICommand value, object parameter, CultureInfo culture)
        {
            return value.CanExecute(parameter);
        }
    }
}
