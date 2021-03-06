﻿using System.Globalization;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Docking.Converters
{
    internal class BooleanOrConverter : MultiValueConverter<bool, bool, bool>
    {
        protected override bool Convert(bool value1, bool value2, object parameter, CultureInfo culture)
        {
            return value1 | value2;
        }
    }
}
