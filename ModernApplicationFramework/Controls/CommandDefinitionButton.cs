using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Test;

namespace ModernApplicationFramework.Controls
{
    public class CommandDefinitionButton : System.Windows.Controls.Button, INotifyPropertyChanged
    {
        private object _icon;
        private readonly object _iconSource;

        public CommandDefinitionButton(CommandBarItemDefinitionBase definition)
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
            DataContext = definition;

            if (string.IsNullOrEmpty(definition.CommandDefinition.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = definition.CommandDefinition.IconSource };
            _iconSource = myResourceDictionary[definition.CommandDefinition.IconId];
        }

        private void ThemeManager_OnThemeChanged(object sender, Core.Events.ThemeChangedEventArgs e)
        {
            SetIcon();
        }

        public void SetIcon()
        {
            if (_iconSource == null)
                return;
            var vb = _iconSource as Viewbox;
            var i = ImageUtilities.IconImageFromFrameworkElement(vb);
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);

            var b = ImageUtilities.BitmapFromBitmapSource((BitmapSource)i.Source);
            var bi = ImageThemingUtilities.GetThemedBitmap(b, ImageThemingUtilities.GetImageBackgroundColor(this).ToRgba());
            var bs = ImageConverter.BitmapSourceFromBitmap(bi);
            i.Source = bs;
            Icon = i;
        }

        public object Icon
        {
            get => _icon;
            set
            {
                if (Equals(value, _icon)) return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
