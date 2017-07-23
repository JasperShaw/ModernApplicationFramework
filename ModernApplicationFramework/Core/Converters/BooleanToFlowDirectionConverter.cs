using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter{TSource,TTarget}"/> that converts a <see langword="true"/> to <see cref="FlowDirection.RightToLeft"/>
    /// </summary>
    public class BooleanToFlowDirectionConverter : ValueConverter<bool, FlowDirection>
    {
        protected override FlowDirection Convert(bool value, object parameter, CultureInfo culture)
        {
            var flowDirection = FlowDirection.LeftToRight;
            if (value)
                flowDirection = FlowDirection.RightToLeft;
            return flowDirection;
        }
    }
}