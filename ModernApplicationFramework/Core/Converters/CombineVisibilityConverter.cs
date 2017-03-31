using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernApplicationFramework.Core.Converters
{
    public class CombineVisibilityConverter : IMultiValueConverter
    {

        private CombineVisibility _combineVisibility;

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
            if (((IEnumerable<object>)values).Contains<object>(DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;
            IEnumerable<Visibility> source = values.Cast<Visibility>();
            switch (this._combineVisibility)
            {
                case CombineVisibility.PickHighestVisibility:
                    return (object)source.Min<Visibility>();
                case CombineVisibility.PickLowestVisibility:
                    return (object)source.Max<Visibility>();
                default:
                    throw new InvalidOperationException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public enum CombineVisibility
    {
        PickHighestVisibility,
        PickLowestVisibility,
    }
}
