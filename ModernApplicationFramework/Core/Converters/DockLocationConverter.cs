using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.Converters
{
    internal class DockLocationConverter : ValueConverter<uint, Dock>
    {
        protected override Dock Convert(uint value, object parameter, CultureInfo culture)
        {
            return (Dock) value;
        }

        protected override uint ConvertBack(Dock value, object parameter, CultureInfo culture)
        {
            return (uint) value;
        }
    }
}
