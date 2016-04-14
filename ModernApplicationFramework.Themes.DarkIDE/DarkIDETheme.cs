using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes.DarkIDE
{
    [Export(typeof(Theme))]
    public class DarkTheme : Theme
    {
        public override string Name => "Dark";

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework.Themes.DarkIDE;component/DarkIDETheme.xaml", UriKind.Relative);
        }
    }
}
