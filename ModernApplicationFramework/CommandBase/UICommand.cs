using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernApplicationFramework.Annotations;
using Action = System.Action;

namespace ModernApplicationFramework.CommandBase
{
    public class UICommand : AbstractCommandWrapper, INotifyPropertyChanged
    {
        private MultiKeyGesture _keyGesture;
        private CommandGestureCategory _category;
        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// The key gesture
        /// </summary>
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

        public CommandGestureCategory Category
        {
            get => _category;
            set
            {
                if (Equals(value, _category))
                    return;
                _category = value;
                OnPropertyChanged();
            }
        }

        public UICommand(Action executeAction, Func<bool> cantExectueFunc) : base(executeAction, cantExectueFunc)
        {
            Category = CommandGestureCategories.GlobalGestureCategory;
        }


        public UICommand(ICommand wrappedCommand) : this(wrappedCommand, null)
        {
        }
        
        public UICommand(ICommand wrappedCommand, MultiKeyGesture gesture) : base(wrappedCommand)
        {
            if (gesture == null)
                return;
            KeyGesture = gesture;
            Category = CommandGestureCategories.GlobalGestureCategory;
        }

        public UICommand(Action executeAction, Func<bool> cantExectueFunc,
            MultiKeyGesture gesture) : base(executeAction, cantExectueFunc)
        {
            if (gesture == null)
                return;
            KeyGesture = gesture;
            Category = CommandGestureCategories.GlobalGestureCategory;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
