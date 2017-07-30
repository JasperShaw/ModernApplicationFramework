using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace ModernApplicationFramework.CommandBase.Input
{
    public class MultiKeyBinding : InputBinding
    {
        [TypeConverter(typeof(MultiKeyGestureConverter))]
        [ValueSerializer(typeof(MultiKeyGestureValueSerializer))]
        public override InputGesture Gesture
        {
            get
            {
                if (base.Gesture == null)
                    base.Gesture = new MultiKeyGesture(Key.None);
                return base.Gesture as MultiKeyGesture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!(value is MultiKeyGesture))
                    throw new AggregateException($"Gesture is not of type {typeof(MultiKeyGesture)}");
                base.Gesture = value;
            }
        }

        public IEnumerable<KeySequence> KeySequences
        {
            get => ((MultiKeyGesture) Gesture).GestureCollection;
            set => Gesture = new MultiKeyGesture(new List<KeySequence>(value));
        }

        public MultiKeyBinding()
        {
            
        }

        public MultiKeyBinding(ICommand command, MultiKeyGesture gesture) : base(command, gesture)
        {
            
        }

        public MultiKeyBinding(ICommand command, IEnumerable<Key> keys, ModifierKeys modifiers)
            : base(command, new MultiKeyGesture(keys, modifiers))
        {
        }
    }
}
