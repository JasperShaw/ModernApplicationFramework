using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls
{
    public class CommandDefinitionButton : System.Windows.Controls.Button, INotifyPropertyChanged, IThemableIconContainer
    {
        private object _icon;
        public object IconSource { get; }

        public CommandDefinitionButton(CommandBarDefinitionBase definition)
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
            DataContext = definition;

            if (string.IsNullOrEmpty(definition.CommandDefinition.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = definition.CommandDefinition.IconSource };
            IconSource = myResourceDictionary[definition.CommandDefinition.IconId];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.SetThemedIcon();
        }

        private void ThemeManager_OnThemeChanged(object sender, Core.Events.ThemeChangedEventArgs e)
        {
            this.SetThemedIcon();
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
