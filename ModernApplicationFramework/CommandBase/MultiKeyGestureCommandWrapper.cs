using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.CommandBase
{
    public class MultiKeyGestureCommandWrapper : CommandWrapper, INotifyPropertyChanged
    {
        private MultiKeyGesture _keyGesture;

        public MultiKeyGestureCommandWrapper(Action executeAction, Func<bool> cantExectueFunc) : base(executeAction, cantExectueFunc)
        {    
        }

        public MultiKeyGestureCommandWrapper(ICommand wrappedCommand, MultiKeyGesture gesture) : base(wrappedCommand)
        {
            if (gesture == null)
                return;
            KeyGesture = gesture;
        }

        public MultiKeyGestureCommandWrapper(Action executeAction, Func<bool> cantExectueFunc,
            MultiKeyGesture gesture) : base(executeAction, cantExectueFunc)
        {
            if (gesture == null)
                return;
            KeyGesture = gesture;
        }

        public string GestureText
        {
            get
            {
                var gesture = KeyGesture;
                if (gesture == null)
                    return null;
                return
                    (string)
                        MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, gesture,
                            typeof(string));
            }
        }

        public MultiKeyGesture KeyGesture
        {
            get => _keyGesture;
            set
            {
                if (_keyGesture == value)
                    return;
                _keyGesture = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GestureText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
