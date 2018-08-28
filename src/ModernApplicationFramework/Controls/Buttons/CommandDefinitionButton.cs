using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Utilities;
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
    public class CommandDefinitionButton : System.Windows.Controls.Button, INotifyPropertyChanged
    {
        private object _icon;
        private PropertyChangedEventHandler _propertyChanged;
        
        /// <summary>
        /// The toolbar this button is inside
        /// </summary>
        public ToolBar ParentToolBar => this.FindAncestor<ToolBar>();

        static CommandDefinitionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandDefinitionButton), new FrameworkPropertyMetadata(typeof(CommandDefinitionButton)));
        }

        public CommandDefinitionButton(CommandBarDataSource definition) : this()
        {
            DataContext = definition;
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
