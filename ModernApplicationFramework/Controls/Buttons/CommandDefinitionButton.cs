using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class CommandDefinitionButton : System.Windows.Controls.Button, INotifyPropertyChanged, IThemableIconContainer
    {
        private object _icon;
        private PropertyChangedEventHandler _propertyChanged;
        public object IconSource { get; }

        public ToolBar ParentToolBar => this.FindAncestor<ToolBar>();

        static CommandDefinitionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandDefinitionButton), new FrameworkPropertyMetadata(typeof(CommandDefinitionButton)));
        }

        public CommandDefinitionButton(CommandBarDefinitionBase definition) : this()
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
            IsEnabledChanged += CommandDefinitionButton_IsEnabledChanged;
            DataContext = definition;

            if (string.IsNullOrEmpty(definition.CommandDefinition.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary {Source = definition.CommandDefinition.IconSource};
            IconSource = myResourceDictionary[definition.CommandDefinition.IconId];
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                var changedEventHandler = _propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref _propertyChanged, (PropertyChangedEventHandler)Delegate.Combine(comparand, value), comparand);
                }
                while (changedEventHandler != comparand);
            }
            remove
            {
                var changedEventHandler = _propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref _propertyChanged, (PropertyChangedEventHandler)Delegate.Remove(comparand, value), comparand);
                }
                while (changedEventHandler != comparand);
            }
        }

        public CommandDefinitionButton()
        {
            DteFocusHelper.HookAcquireFocus(this);
        }

        private void CommandDefinitionButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SetThemedIcon();
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

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!Equals(e.NewFocus, this))
                return;
            var templateChild = GetTemplateChild("PART_FocusTarget") as UIElement;
            templateChild?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            OnPropertyChanged(nameof(ParentToolBar));
            base.OnVisualParentChanged(oldParent);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _propertyChanged.RaiseEvent(this, propertyName);
        }
    }
}
