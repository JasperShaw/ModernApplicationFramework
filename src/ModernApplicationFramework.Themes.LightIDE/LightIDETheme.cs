using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Themes.LightIDE
{
    [Export(typeof(Theme))]
    public class LightTheme : Theme
    {
        public override string Name => "Light";
        public override string Text => "Light";

        public override Uri GetResourceUri()
        {
            return new Uri("/ModernApplicationFramework.Themes.LightIDE;component/LightIDETheme.xaml", UriKind.Relative);
        }
    }
}