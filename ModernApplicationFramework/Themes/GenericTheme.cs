using System;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes
{
    public class GenericTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework;component/Themes/Generic.xaml", UriKind.Relative);
        }
    }
}
