using System.Globalization;
using ModernApplicationFramework.Core.Platform.Enums;

namespace ModernApplicationFramework.Core.Converters
{
    internal class ToolBarVisibilityConverter : ToBooleanValueConverter<int>
    {
        protected override bool Convert(int value, object parameter, CultureInfo culture)
        {
            switch ((ToolBarVisibilityState) value)
            {
                case ToolBarVisibilityState.Undefined:
                    return false;
                case ToolBarVisibilityState.Hidden:
                    return false;
                default:
                    return true;
            }
        }

        protected override int ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value ? 0 : 1;
        }
    }
}