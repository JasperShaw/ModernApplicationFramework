using System;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    internal class WindowStateConverter : ValueConverter<int, WindowState>
    {
        protected override WindowState Convert(int state, object parameter, CultureInfo culture)
        {
            switch (state)
            {
                case 1:
                    return WindowState.Normal;
                case 2:
                    return WindowState.Minimized;
                case 3:
                    return WindowState.Maximized;
                default:
                    throw new ArgumentException("Invalid Window State");
            }
        }

        protected override int ConvertBack(WindowState state, object parameter, CultureInfo culture)
        {
            switch (state)
            {
                case WindowState.Normal:
                    return 1;
                case WindowState.Minimized:
                    return 2;
                case WindowState.Maximized:
                    return 3;
                default:
                    throw new ArgumentException("Invalid Window State");
            }
        }
    }
}