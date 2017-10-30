using System;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Dialogs
{
    internal class TextInputViewModel : ObservableObject
    {
        private string _text;
        private int _maxLength;
        private bool _allowWhiteSpace;
        private readonly Predicate<string> _validator;

        public string Title { get; }

        public string Prompt { get; }

        public string Text
        {
            get => _text;
            set
            {
                if (!SetProperty(ref _text, value))
                    return;
                ((DelegateCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        public bool AllowWhiteSpace
        {
            get => _allowWhiteSpace;
            set
            {
                if (!SetProperty(ref _allowWhiteSpace, value))
                    return;
                ((DelegateCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        public int MaxLength
        {
            get => _maxLength;
            set
            {
                Validate.IsWithinRange(value, 1, int.MaxValue, "MaxLength");
                if (!SetProperty(ref _maxLength, value))
                    return;
                string text = Text;
                if (string.IsNullOrEmpty(text) || text.Length <= value)
                    return;
                Text = text.Substring(0, MaxLength);
            }
        }

        public bool IsCancelled { get; private set; }

        public ICommand SubmitCommand { get; }

        public ICommand CancelCommand { get; }

        internal event EventHandler RequestClose;

        internal TextInputViewModel(string title, string prompt, string defaultText)
            : this(title, prompt, defaultText, null)
        {
        }

        internal TextInputViewModel(string title, string prompt, string defaultText, Predicate<string> validator)
            : this(title, prompt, int.MaxValue, defaultText, validator)
        {
        }

        internal TextInputViewModel(string title, string prompt, int maxLength, string defaultText, Predicate<string> validator)
        {
            Validate.IsNotNullAndNotEmpty(title, "title");
            Validate.IsNotNullAndNotEmpty(prompt, "prompt");
            Validate.IsWithinRange(maxLength, 1, int.MaxValue, "maxLength");
            if (defaultText != null)
                Validate.IsWithinRange(defaultText.Length, 0, maxLength, "defaultText.Length");
            SubmitCommand = new DelegateCommand(OnSubmitCommandExecuted, CanExecuteSubmitCommand);
            CancelCommand = new DelegateCommand(OnCancelCommandExecuted);
            IsCancelled = true;
            Title = title;
            Prompt = prompt;
            Text = defaultText;
            MaxLength = maxLength;
            _validator = validator;
        }

        private void OnSubmitCommandExecuted(object parameter)
        {
            if (_validator != null && !_validator(Text))
                return;
            IsCancelled = false;
            RequestClose.RaiseEvent(this);
        }

        private void OnCancelCommandExecuted(object parameter)
        {
            IsCancelled = true;
            // ISSUE: reference to a compiler-generated field
            RequestClose.RaiseEvent(this);
            Text = null;
        }

        private bool CanExecuteSubmitCommand(object parameter)
        {
            if (!AllowWhiteSpace)
                return !string.IsNullOrWhiteSpace(Text);
            return true;
        }
    }
}
