using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public abstract class ViewPresenter : LayoutSynchronizedContentControl, ILayoutItemHost
    {
        private DependencyObject _currentFocusScope;

        public static readonly RoutedEvent ContentShowingEvent = EventManager.RegisterRoutedEvent("ContentShowing",
            RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));

        public static readonly RoutedEvent ContentHidingEvent = EventManager.RegisterRoutedEvent("ContentHiding",
            RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));

        public static readonly DependencyProperty CanActivateFromLeftClickProperty =
            DependencyProperty.RegisterAttached("CanActivateFromLeftClick", typeof(bool), typeof(ViewPresenter),
                new FrameworkPropertyMetadata(Boxes.BooleanTrue));

        public static readonly DependencyProperty CanActivateFromMiddleClickProperty =
            DependencyProperty.RegisterAttached("CanActivateFromMiddleClick", typeof(bool), typeof(ViewPresenter),
                new FrameworkPropertyMetadata(Boxes.BooleanTrue));

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutContent), typeof(ViewPresenter),
                new FrameworkPropertyMetadata(null, OnModelChanged));


        public LayoutContent Model
        {
            get => (LayoutContent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }


        public abstract LayoutItem LayoutItem { get; }

        static ViewPresenter()
        {
            IsTabStopProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        protected ViewPresenter()
        {
            IsVisibleChanged += OnIsVisibleChanged;
            UtilityMethods.AddPresentationSourceCleanupAction(this, () =>
            {
                if (_currentFocusScope == null)
                    return;
                FocusManager.SetFocusedElement(_currentFocusScope, null);
            });
            AccessKeyManager.AddAccessKeyPressedHandler(this, OnAccessKeyPressed);
        }

        public static bool GetCanActivateFromLeftClick(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool)element.GetValue(CanActivateFromLeftClickProperty);
        }

        public static void SetCanActivateFromLeftClick(DependencyObject element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CanActivateFromLeftClickProperty, Boxes.Box(value));
        }

        public static bool GetCanActivateFromMiddleClick(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool)element.GetValue(CanActivateFromMiddleClickProperty);
        }

        public static void SetCanActivateFromMiddleClick(DependencyObject element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CanActivateFromMiddleClickProperty, Boxes.Box(value));
        }

        protected abstract void OnModelChanged(DependencyPropertyChangedEventArgs e);

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
                return;
            _currentFocusScope = FocusManager.GetFocusScope(this);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property != DataContextProperty || !IsVisible)
                return;
            var oldValue = e.OldValue as LayoutContent;
            var newValue = e.NewValue as LayoutContent;
            if (oldValue != null)
                AsyncRaiseEvent(new ViewEventArgs(ContentHidingEvent, oldValue));
            if (newValue == null)
                return;
            AsyncRaiseEvent(new ViewEventArgs(ContentShowingEvent, newValue));
        }

        private void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            e.Scope = Model;
            e.Handled = true;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            bool newValue = (bool)args.NewValue;
            if (!(DataContext is LayoutContent dataContext))
                return;
            AsyncRaiseEvent(new ViewEventArgs(newValue ? ContentShowingEvent : ContentHidingEvent, dataContext));
        }

        private void AsyncRaiseEvent(RoutedEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)(() =>
           {
               if (this.IsConnectedToPresentationSource())
                   RaiseEvent(args);
           }));
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewPresenter)d).OnModelChanged(e);
        }
    }
}