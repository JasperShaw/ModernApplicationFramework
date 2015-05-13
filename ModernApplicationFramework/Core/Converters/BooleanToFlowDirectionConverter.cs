using System.Globalization;
using System.Windows;

namespace ModernApplicationFramework.Core.Converters
{
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
