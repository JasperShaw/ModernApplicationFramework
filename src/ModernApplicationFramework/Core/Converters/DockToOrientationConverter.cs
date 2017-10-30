using System;
using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that converts a <see cref="Dock"/> value to a <see cref="Orientation"/> value
    /// </summary>
    public class DockToOrientationConverter : ValueConverter<Dock, Orientation>
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
