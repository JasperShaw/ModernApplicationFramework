﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IMultiValueConverter"/> that returns <see cref="Visibility.Visible"/> if one <see cref="Visibility"/> of a list is <see cref="Visibility.Visible"/>
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IMultiValueConverter" />
    public class MultiVisibilityToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            foreach (var obj in values)
            {
                if (obj == DependencyProperty.UnsetValue)
                    flag = true;
                else if ((Visibility)obj == Visibility.Visible) 
                    return Visibility.Visible;
            }
            return !flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
