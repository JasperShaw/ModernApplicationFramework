using System.Globalization;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.General
{
    internal class GoToLabelConverter : MultiValueConverter<int, int, string>
    {
        protected override string Convert(int value1, int value2, object parameter, CultureInfo culture)
        {
            return string.Format(GotoDialogResourcesResources.GoToLineLabel, value1, value2);
        }
    }
}