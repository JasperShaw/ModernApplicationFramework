using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Core.Converters;

namespace ModernApplicationFramework.Core.Utilities
{
    class BooleanOrConverter : MultiValueConverter<bool, bool, bool>
    {
        protected override bool Convert(bool value1, bool value2, object parameter, CultureInfo culture)
        {
            return value1 || value2;
        }
    }
}
