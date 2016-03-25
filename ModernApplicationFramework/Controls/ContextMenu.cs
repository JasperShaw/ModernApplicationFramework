using System;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenu : System.Windows.Controls.ContextMenu, IChangeTheme
    {
        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
        }

        public event EventHandler OnThemeChanged;
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
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = newTheme.GetResourceUri() });
            }
        }
    }
}
