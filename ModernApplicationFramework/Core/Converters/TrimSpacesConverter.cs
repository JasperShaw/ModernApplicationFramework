using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Core.Converters
{
    internal class TrimSpacesConverter : ValueConverter<string, string>
    {
        protected override string Convert(string value, object parameter, CultureInfo culture)
        {
            return value.Trim();
        }

        protected override string ConvertBack(string value, object parameter, CultureInfo culture)
        {
            return value.Trim();
        }
    }
}
