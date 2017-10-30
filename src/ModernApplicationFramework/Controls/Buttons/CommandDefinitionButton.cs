using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc cref="System.Windows.Controls.Button" />
    /// <summary>
    /// A command bar button supporting themable icons
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IThemableIconContainer" />
    public class CommandDefinitionButton : System.Windows.Controls.Button, INotifyPropertyChanged, IThemableIconContainer
    {
        private object _icon;
        private PropertyChangedEventHandler _propertyChanged;
        
        /// <inheritdoc />
        /// <summary>
        /// This is the original source to icon. Usually this is a resource containing a <see cref="T:System.Windows.Controls.Viewbox" /> element.
        /// </summary>
        public object IconSource { get; }

        /// <summary>
        /// The toolbar this button is inside
        /// </summary>
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
