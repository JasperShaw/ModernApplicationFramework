using System;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Modules.Toolbox.Converters
{
    internal class CategoryVisibilityConverter : MultiValueConverter<bool, bool, bool, bool, Guid, Visibility>
    {
        protected override Visibility Convert(bool value1, bool value2, bool value3, bool value4, Guid value5, object parameter, CultureInfo culture)
        {
            if (value1 || value2 || value3 ||value4 || value5.Equals(Guids.DefaultCategoryId))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
    }
}
