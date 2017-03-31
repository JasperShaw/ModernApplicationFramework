using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class Button : System.Windows.Controls.Button, INotifyPropertyChanged
    {

        private PropertyChangedEventHandler _propertyChanged;

        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
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

        public Button()
        {
            DteFocusHelper.HookAcquireFocus(this);
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
            NotifyPropertyChanged("ParentToolBar");
            base.OnVisualParentChanged(oldParent);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            _propertyChanged.RaiseEvent(this, propertyName);
        }
    }
}