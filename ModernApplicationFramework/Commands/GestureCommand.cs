using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Commands
{
    public class GestureCommand : Command, INotifyPropertyChanged
    {
        public GestureCommand(Action executeMethod, KeyGesture gesture) : base(executeMethod)
        {
            KeyGesture = gesture;
        }

        public GestureCommand(Action executeMethod, Func<bool> canExecuteMethod, KeyGesture gesture) : base(executeMethod, canExecuteMethod)
        {
            KeyGesture = gesture;
        }

        private KeyGesture _keyGesture;

        public KeyGesture KeyGesture
        {
            get { return _keyGesture; }
            set
            {
                if (_keyGesture == value)
                    return;
                _keyGesture = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GestureText));
            }
        }

        public string GestureText => KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
