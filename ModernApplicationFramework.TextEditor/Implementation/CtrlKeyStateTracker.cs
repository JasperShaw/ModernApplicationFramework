using System;
using System.Windows.Input;
using ModernApplicationFramework.Utilities.Core;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal sealed class CtrlKeyStateTracker
    {
        public event EventHandler<EventArgs> CtrlKeyStateChanged;

        internal bool CheckCtrlKeyState = true;
        private bool _isCtrlKeyDown;

        public KeyProcessor KeyProcessor { get; }

        public bool IsCtrlKeyDown
        {
            get
            {
                if (CheckCtrlKeyState)
                {
                    var flag = (Keyboard.Modifiers & ModifierKeys.Control) > 0U;
                    if (flag != _isCtrlKeyDown)
                        IsCtrlKeyDown = false;
                }
                return _isCtrlKeyDown;
            }
            set
            {
                if (value == _isCtrlKeyDown)
                    return;
                _isCtrlKeyDown = value;
                CtrlKeyStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private CtrlKeyStateTracker()
        {
            _isCtrlKeyDown = (Keyboard.Modifiers & ModifierKeys.Control) > 0U;
            KeyProcessor = new CtrlKeyProcessor(this);
        }

        public static CtrlKeyStateTracker GetStateTrackerForView(IPropertyOwner view)
        {
            return view.Properties.GetOrCreateSingletonProperty(typeof(CtrlKeyStateTracker),
                () => new CtrlKeyStateTracker());
        }

        private class CtrlKeyProcessor : KeyProcessor
        {
            private readonly CtrlKeyStateTracker _state;

            public CtrlKeyProcessor(CtrlKeyStateTracker state)
            {
                _state = state;
            }

            public override void PreviewKeyDown(KeyEventArgs args)
            {
                UpdateState(args);
            }

            public override void PreviewKeyUp(KeyEventArgs args)
            {
                UpdateState(args);
            }

            private void UpdateState(KeyboardEventArgs args)
            {
                _state.IsCtrlKeyDown = (args.KeyboardDevice.Modifiers & ModifierKeys.Control) > 0U;
            }
        }
    }
}