using System;
using System.Globalization;
using System.Windows.Data;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Core.Converters
{
    internal sealed class ToolBarVisibilityConverter: IValueConverter
    {
        public bool Convert(int value, object parameter, CultureInfo culture)
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

        public int ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value ? 0 : 1;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ToolBarVisibilityState)value)
            {
                case ToolBarVisibilityState.Undefined:
                    return false;
                case ToolBarVisibilityState.Hidden:
                    return false;
                default:
                    return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? ToolBarVisibilityState.Hidden : ToolBarVisibilityState.Visible;
        }
    }
}