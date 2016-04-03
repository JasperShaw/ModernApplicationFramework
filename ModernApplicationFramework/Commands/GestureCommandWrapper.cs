using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Commands
{
    public class GestureCommandWrapper : CommandWrapper, INotifyPropertyChanged
    {
        private KeyGesture _keyGesture;

        public GestureCommandWrapper(ICommand wrappedCommand, KeyGesture gesture) : base(wrappedCommand)
        {
            KeyGesture = gesture;
        }

        public GestureCommandWrapper(Action executeAction, Func<bool> cantExectueFunc, KeyGesture gesture)
            : base(executeAction, cantExectueFunc)
        {
            KeyGesture = gesture;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string GestureText => KeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}