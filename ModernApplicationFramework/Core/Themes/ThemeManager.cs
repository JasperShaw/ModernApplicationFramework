using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Core.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:ModernApplicationFramework.Core.Themes.ThemeManager" /> manages the current UI's theme
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Services.IThemeManager" />
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        protected const string RegistryKey = "InstalledTheme";
        protected const string DefaultThemeName = "Blue";

        private readonly Theme[] _themes;
        private Theme _theme;

        /// <summary>
        /// Fired when the current theme is changed
        /// </summary>
        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        /// <inheritdoc />
        /// <summary>
        /// A list of all installed themes
        /// </summary>
        public IEnumerable<Theme> Themes => _themes;

        /// <inheritdoc />
        /// <summary>
        /// Gets the theme saved when re-launching the environment
        /// </summary>
        public Theme StartUpTheme
        {
            get
            {
                var theme = IoC.Get<IEnvironmentVariables>().GetOrCreateRegistryVariable(RegistryKey, null, DefaultThemeName);
                return _themes.FirstOrDefault(x => x.Name == theme);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The current theme of the environment
        /// </summary>
        /// <exception cref="T:System.Data.NoNullAllowedException"></exception>
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

        [ImportingConstructor]
        public ThemeManager([ImportMany] Theme[] themes)
        {
            _themes = themes;
        }

        /// <inheritdoc />
        /// <summary>
        /// Saves a theme for re-launch
        /// </summary>
        /// <param name="theme">The theme that should be saved</param>
        public void SaveTheme(Theme theme)
        {
            IoC.Get<IEnvironmentVariables>().SetRegistryVariable(RegistryKey, theme.Name, null);
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
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