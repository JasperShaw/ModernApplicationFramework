using System.Globalization;
using ModernApplicationFramework.Modules.Toolbox.Controls;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Modules.Toolbox.Converters
{
    internal class TreeNodeEditModeWidthConverter : ValueConverter<ToolboxTreeView, double>
    {
        protected override double Convert(ToolboxTreeView value, object parameter, CultureInfo culture)
        {
            return value.ActualWidth - 19 - 10;
        }
    }
}
