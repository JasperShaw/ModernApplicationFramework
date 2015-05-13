using System;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes.LightIDE
{
    public class LightTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework.Themes.LightIDE;component/LightIDETheme.xaml", UriKind.Relative);
        }
    }
}
