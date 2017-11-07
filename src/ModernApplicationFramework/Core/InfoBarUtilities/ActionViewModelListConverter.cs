using System.Collections.Generic;
using System.Globalization;
using ModernApplicationFramework.Basics.InfoBar.Internal;
using ModernApplicationFramework.Controls.InfoBar.Internal;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.InfoBarUtilities
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
