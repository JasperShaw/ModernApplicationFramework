using System;
using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class DockToOrienationConverter : ValueConverter<Dock, Orientation>
    {
        protected override Orientation Convert(Dock value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Dock.Left:
                case Dock.Right:
                    return Orientation.Vertical;
                case Dock.Top:
                case Dock.Bottom:
                    return Orientation.Horizontal;
            }
            throw new ArgumentException();
        }
    }
}
