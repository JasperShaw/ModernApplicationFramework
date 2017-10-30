using System;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes
{
    public class GenericTheme : Theme
    {
        public override string Name => "Generic";
        public override string Text => "Generic";

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework;component/Themes/Generic.xaml", UriKind.Relative);
        }
    }
}