using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Core.Themes
{
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        protected const string RegistryKey = "InstalledTheme";
        protected const string DefaultThemeName = "Blue";

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
            IoC.Get<IEnvironmentVarirables>().SetRegistryVariable(RegistryKey, theme.Name, null);
        }

        public Theme StartUpTheme
        {
            get
            {
                var theme = IoC.Get<IEnvironmentVarirables>().GetOrCreateRegistryVariable(RegistryKey, null, DefaultThemeName);
                return _themes.FirstOrDefault(x => x.Name == theme);
            }
        }

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