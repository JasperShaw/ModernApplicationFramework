using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Themes
{
    [Export(typeof(Theme))]
    public class BlueTheme : Theme
    {
        public override string Name => CommonUI_Resources.ThemeBlue_Name;

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework;component/Themes/BlueTheme.xaml", UriKind.Relative);
        }
    }
}