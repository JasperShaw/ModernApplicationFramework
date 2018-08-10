using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class EditorAndMenuFocusTracker
    {
        internal Func<ITextView, bool> IsKeyboardFocusWithin = view => view.VisualElement.IsKeyboardFocusWithin;
        internal Func<bool> InputManagerCurrentIsInMenuMode = () => InputManager.Current.IsInMenuMode;
        internal Func<IInputElement> KeyboardFocusedElement = () => Keyboard.FocusedElement;

        private readonly ITextView _textView;
        private FocusState _currentState;

        public event EventHandler GotFocus;

        public event EventHandler LostFocus;

        public EditorAndMenuFocusTracker(ITextView textView)
        {
            _textView = textView;
            _textView.GotAggregateFocus += OnGotAggregateFocus;
            _textView.LostAggregateFocus += OnLostAggregateFocus;
            _textView.Closed += OnClosed;
            _currentState = _textView.HasAggregateFocus ? FocusState.EditorFocused : FocusState.NotFocused;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _textView.GotAggregateFocus -= OnGotAggregateFocus;
            _textView.LostAggregateFocus -= OnLostAggregateFocus;
            _textView.Closed -= OnClosed;
            InputManager.Current.LeaveMenuMode -= OnLeaveMenuMode;
        }

        private void OnGotAggregateFocus(object sender, EventArgs e)
        {
            if (_currentState == FocusState.NotFocused)
            {
                _currentState = FocusState.EditorFocused;
                RaiseGotFocus();
            }
            else if (_currentState == FocusState.FocusReturning_WaitingForGotAggregateFocus)
                _currentState = FocusState.EditorFocused;
            else
            {
                if (_currentState != FocusState.MenuFocused)
                    return;
                _currentState = FocusState.FocusReturning_WaitingForLeaveMenuMode;
            }
        }

        private void OnLostAggregateFocus(object sender, EventArgs e)
        {
            if (!InputManagerCurrentIsInMenuMode() || KeyboardFocusedElement() is ComboBox comboBox && comboBox.IsEditable)
            {
                _currentState = FocusState.NotFocused;
                RaiseLostFocus();
            }
            else
            {
                _currentState = FocusState.MenuFocused;
                InputManager.Current.LeaveMenuMode += OnLeaveMenuMode;
            }
        }

        private void OnLeaveMenuMode(object sender, EventArgs e)
        {
            InputManager.Current.LeaveMenuMode -= OnLeaveMenuMode;
            if (_currentState == FocusState.FocusReturning_WaitingForLeaveMenuMode)
                _currentState = FocusState.EditorFocused;
            else if (_currentState == FocusState.MenuFocused)
            {
                if (!IsKeyboardFocusWithin(_textView))
                {
                    _currentState = FocusState.NotFocused;
                    RaiseLostFocus();
                }
                else
                    _currentState = FocusState.FocusReturning_WaitingForGotAggregateFocus;
            }
            else
            {
                _currentState = FocusState.NotFocused;
                RaiseLostFocus();
            }
        }

        private void RaiseGotFocus()
        {
            GotFocus?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseLostFocus()
        {
            LostFocus?.Invoke(this, EventArgs.Empty);
        }


        private enum FocusState
        {
            NotFocused,
            EditorFocused,
            MenuFocused,
            FocusReturning_WaitingForLeaveMenuMode,
            FocusReturning_WaitingForGotAggregateFocus,
        }
    }
}