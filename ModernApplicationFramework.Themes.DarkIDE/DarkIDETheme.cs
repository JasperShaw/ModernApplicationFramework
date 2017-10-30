using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Themes.DarkIDE.Properties;

namespace ModernApplicationFramework.Themes.DarkIDE
{
    [Export(typeof(Theme))]
    public class DarkTheme : Theme
    {
        public override string Name => "Dark";
        public override string Text => Resources.ThemeDark_Name;

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework.Themes.DarkIDE;component/DarkIDETheme.xaml", UriKind.Relative);
        }
    }
}
