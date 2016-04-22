using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Themes
{
    [Export(typeof(ThemeManager))]
    public class ThemeManager
    {
        private readonly Theme[] _themes;

        [ImportingConstructor]
        public ThemeManager([ImportMany] Theme[] themes)
        {
            _themes = themes;
        }

        public IEnumerable<Theme> Themes => _themes;

        public void SetTheme(string currentTheme, IHasTheme element)
        {
            var theme = _themes.FirstOrDefault(x => x.Name == currentTheme);
            element.Theme = theme;
        }

        public void SaveTheme(string name)
        {
            Settings.Default.CurrentTheme = name;
            Settings.Default.Save();
        }

        public Theme GetCurrentTheme()
        {
            return _themes.FirstOrDefault(x => x.Name == Settings.Default.CurrentTheme);
        }
    }
}