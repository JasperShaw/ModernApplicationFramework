using System;
using System.Globalization;
using System.Windows.Input;

namespace ModernApplicationFramework.Commands
{
    public class MultiKeyGestureCommandWrapper : GestureCommandWrapper
    {
        public MultiKeyGestureCommandWrapper(ICommand wrappedCommand, KeyGesture gesture) : base(wrappedCommand, gesture) {}
        public MultiKeyGestureCommandWrapper(Action executeAction, Func<bool> cantExectueFunc, KeyGesture gesture) : base(executeAction, cantExectueFunc, gesture) {}

        public override string GestureText
        {
            get
            {
                var gesture = KeyGesture as MultiKeyGesture;
                if (gesture == null)
                    return null;
                return
                    (string)
                        MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, gesture,
                            typeof(string));
            }
        }
    }
}
