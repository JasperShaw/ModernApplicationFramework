using System;
using System.Data;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenu : System.Windows.Controls.ContextMenu, IHasTheme
    {
        private Theme _theme;

        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu),
                new FrameworkPropertyMetadata(typeof(ContextMenu)));
        }

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        public Theme Theme
        {
            get { return _theme; }
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
            }
        }

        public void ChangeTheme(Theme oldValue, Theme newValue)
        {
            var oldTheme = oldValue;
            var newTheme = newValue;
            var resources = Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }

            if (newTheme != null)
            {
                resources.MergedDictionaries.Add(new ResourceDictionary {Source = newTheme.GetResourceUri()});
            }
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }
    }
}