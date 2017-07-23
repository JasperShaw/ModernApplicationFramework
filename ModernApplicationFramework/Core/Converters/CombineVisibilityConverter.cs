using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IMultiValueConverter"/> that converts a list of <see cref="Visibility"/> 
    /// to either the lowest or highest <see cref="Visibility"/> in that list
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IMultiValueConverter" />
    public class CombineVisibilityConverter : IMultiValueConverter
    {

        private CombineVisibility _combineVisibility;

        /// <summary>
        /// The visibility option
        /// </summary>
        public CombineVisibility CombineVisibility
        {
            get => _combineVisibility;
            set
            {
                if (value != CombineVisibility.PickHighestVisibility && value != CombineVisibility.PickLowestVisibility)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _combineVisibility = value;
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;
            IEnumerable<Visibility> source = values.Cast<Visibility>();
            switch (_combineVisibility)
            {
                case CombineVisibility.PickHighestVisibility:
                    return source.Min();
                case CombineVisibility.PickLowestVisibility:
                    return source.Max();
                default:
                    throw new InvalidOperationException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// The option enumeration
    /// </summary>
    public enum CombineVisibility
    {
        PickHighestVisibility,
        PickLowestVisibility,
    }
}
