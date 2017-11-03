using System.Collections.Generic;
using System.Globalization;
using ModernApplicationFramework.Controls.InfoBar.Internal;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal class ActionViewModelListConverter : ValueConverter<InfoBarActionViewModel, IEnumerable<InfoBarTextViewModel>>
    {
        protected override IEnumerable<InfoBarTextViewModel> Convert(InfoBarActionViewModel value, object parameter, CultureInfo culture)
        {
            return new[]
            {
                value
            };
        }
    }
}
