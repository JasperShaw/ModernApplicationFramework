using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.EditorBase.Core.Converters
{
    internal class ProviderTreeViewVisibilityConverter : ValueConverter<bool, GridLength>
    {
        protected override GridLength Convert(bool value, object parameter, CultureInfo culture)
        {
            if (!int.TryParse(parameter.ToString(), out var lenght))
                return new GridLength(0);
            return value ? new GridLength(lenght) : new GridLength(0);
        }
    }
}
