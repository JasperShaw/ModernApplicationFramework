using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class DropDownButton : ContentControl, ICommandSource
    {
        private const string PartDropDownButton = "PART_DropDownButton";
        private const string PartContentPresenter = "PART_ContentPresenter";
        private const string PartPopup = "PART_Popup";

        public static readonly DependencyProperty DropDownContentProperty =
            DependencyProperty.Register("DropDownContent", typeof(object), typeof(DropDownButton),
                new UIPropertyMetadata(null, OnDropDownContentChanged));

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool),
            typeof(DropDownButton), new UIPropertyMetadata(false, OnIsOpenChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DropDownButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget",
            typeof(IInputElement), typeof(DropDownButton), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand), typeof(DropDownButton), new PropertyMetadata(null, OnCommandChanged));


        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(DropDownButton));

        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButton));

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButton));

        private ButtonBase _button;
        private EventHandler _canExecuteChangedHandler;

        #region Base Class Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button = GetTemplateChild(PartDropDownButton) as ToggleButton;
            _contentPresenter = GetTemplateChild(PartContentPresenter) as ContentPresenter;

            if (_popup != null)
                _popup.Opened -= Popup_Opened;
            _popup = GetTemplateChild(PartPopup) as Popup;
            if (_popup != null)
                _popup.Opened += Popup_Opened;
        }

        #endregion

        #region Members

        private ContentPresenter _contentPresenter;
        private Popup _popup;

        #endregion

        #region Constructors

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
                new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public DropDownButton()
        {
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        }

        #endregion

        #region Properties

        protected ButtonBase Button
        {
            get => _button;
            set
            {
                if (_button != null)
                    _button.Click -= DropDownButton_Click;
                _button = value;
                if (_button != null)
                    _button.Click += DropDownButton_Click;
            }
        }

        public object DropDownContent
        {
            get => GetValue(DropDownContentProperty);
            set => SetValue(DropDownContentProperty, value);
        }

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        #endregion

        #region Events

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public event RoutedEventHandler Opened
        {
            add => AddHandler(OpenedEvent, value);
            remove => RemoveHandler(OpenedEvent, value);
        }

        public event RoutedEventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        #endregion

        #region Event Handlers

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsOpen)
            {
                if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
                    return;
                IsOpen = true;
                e.Handled = true;
            }
            else
            {
                if (KeyboardUtilities.IsKeyModifyingPopupState(e))
                {
                    CloseDropDown(true);
                    e.Handled = true;
                }
                else
                    if (e.Key == Key.Escape)
                    {
                        CloseDropDown(true);
                        e.Handled = true;
                    }
            }
        }

        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            CloseDropDown(false);
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged();
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            _contentPresenter?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        #endregion

        #region Methods

        private void CanExecuteChanged()
        {
            if (Command != null)
            {
                var command = Command as RoutedCommand;
                IsEnabled = command?.CanExecute(CommandParameter, CommandTarget) ?? Command.CanExecute(CommandParameter);
            }
        }

        private void CloseDropDown(bool isFocusOnButton)
        {
            if (IsOpen)
                IsOpen = false;
            ReleaseMouseCapture();
            if (isFocusOnButton)
                Button.Focus();
        }

        protected virtual void OnClick()
        {
            RaiseRoutedEvent(ClickEvent);
            RaiseCommand();
        }

        private void RaiseRoutedEvent(RoutedEvent routedEvent)
        {
            var args = new RoutedEventArgs(routedEvent, this);
            RaiseEvent(args);
        }

        private void RaiseCommand()
        {
            if (Command != null)
            {
                var routedCommand = Command as RoutedCommand;
                if (routedCommand == null)
                    Command.Execute(CommandParameter);
                else
                    routedCommand.Execute(CommandParameter, CommandTarget);
            }
        }

        private void UnhookCommand(ICommand oldCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        private void HookUpCommand(ICommand newCommand)
        {
            var handler = new EventHandler(CanExecuteChanged);
            _canExecuteChangedHandler = handler;
            if (newCommand != null)
                newCommand.CanExecuteChanged += _canExecuteChangedHandler;
        }

        private static void OnDropDownContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = o as DropDownButton;
            dropDownButton?.OnDropDownContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = o as DropDownButton;
            dropDownButton?.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnDropDownContentChanged(object p1, object p2) {}

        protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
        {
            RaiseRoutedEvent(newValue ? OpenedEvent : ClosedEvent);
        }

        #endregion

        #region ICommandSource Implementation

        [TypeConverter(typeof(CommandConverter))]
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IInputElement CommandTarget
        {
            get => (IInputElement) GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = o as DropDownButton;
            dropDownButton?.OnCommandChanged((ICommand) e.OldValue, (ICommand) e.NewValue);
        }

        private void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            if (oldValue != null)
                UnhookCommand(oldValue);
            HookUpCommand(newValue);
            CanExecuteChanged();
        }

        #endregion
    }
}