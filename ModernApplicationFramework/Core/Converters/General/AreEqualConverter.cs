﻿using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    public class AreEqualConverter : MultiValueConverter<object, object, bool>
    {
        protected override bool Convert(object value1, object value2, object parameter, CultureInfo culture)
        {
            if (Equals(value1, value2))
                return true;
            return value1 != null && value1.Equals(value2);
        }
    }
}
