using System.Globalization;
using ModernApplicationFramework.Core.Converters.General;
using ModernApplicationFramework.Docking;

namespace MordernApplicationFramework.WindowManagement.Options
{
    internal class DockPreferenceToBooleanConverter : ToBooleanValueConverter<DockPreference>
    {
        protected override bool Convert(DockPreference value, object parameter, CultureInfo culture)
        {
            return value == DockPreference.DockAtEnd;
        }

        protected override DockPreference ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value ? DockPreference.DockAtBeginning : DockPreference.DockAtEnd;
        }
    }
}
