using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Themes.LightIDE.Properties;

namespace ModernApplicationFramework.Themes.LightIDE
{
    [Export(typeof(Theme))]
    public class LightTheme : Theme
    {
        public override string Name => "Light";
        public override string Text => Resources.ThemeLight_Name;

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework.Themes.LightIDE;component/LightIDETheme.xaml", UriKind.Relative);
        }
    }
}