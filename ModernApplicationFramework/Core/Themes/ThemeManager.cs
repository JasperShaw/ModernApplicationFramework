using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Themes
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        private readonly Theme[] _themes;
        private Theme _theme;

        [ImportingConstructor]
        public ThemeManager([ImportMany] Theme[] themes)
        {
            _themes = themes;
        }

        public IEnumerable<Theme> Themes => _themes;

        public void SaveTheme(Theme theme)
        {
            Settings.Default.CurrentTheme = theme.Name;
            Settings.Default.Save();
        }

        public Theme StartUpTheme => _themes.FirstOrDefault(x => x.Name == Settings.Default.CurrentTheme);

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        public Theme Theme
        {
            get => _theme;
            set
            {
                if (value == null)
                    throw new NoNullAllowedException();
                if (Equals(value, _theme))
                    return;
                var oldTheme = _theme;
                _theme = value;
                ChangeTheme(oldTheme, _theme);
                OnRaiseThemeChanged(new ThemeChangedEventArgs(value, oldTheme));
                SaveTheme(_theme);
            }
        }

        private void ChangeTheme(Theme oldTheme, Theme theme)
        {
            var resources = Application.Current.Resources;
            resources.Clear();
            resources.MergedDictionaries.Clear();
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(resourceDictionaryToRemove);
            }
            if (theme != null)
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = theme.GetResourceUri() });
        }
    }
}