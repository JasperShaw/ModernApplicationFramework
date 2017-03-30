using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes
{
    [Export(typeof(Theme))]
    public class BlueTheme : Theme
    {
        public override string Name => "Blue";

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework;component/Themes/BlueTheme.xaml", UriKind.Relative);
        }
    }
}