using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace ModernApplicationFramework.CommandBase.Input
{
    //http://kent-boogaart.com/blog/multikeygesture

    /// <inheritdoc />
    /// <summary>
    /// <see cref="KeyGesture"/> that allows multiple hot keys
    /// </summary>
    /// <seealso cref="KeyGesture" />
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    public class MultiKeyGesture : KeyGesture
    {
        private static readonly TimeSpan MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);

        /// <summary>
        /// An instance of a <see cref="MultiKeyGestureConverter"/>
        /// </summary>
        public static TypeConverter KeyGestureConverter = new MultiKeyGestureConverter();

        private readonly IList<Key> _keys;
        private readonly ReadOnlyCollection<Key> _readOnlyKeys;
        private int _currentKeyIndex;
        private DateTime _lastKeyPress;

        public new string DisplayString => (string) KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, this,
            typeof(string));

        public MultiKeyGesture(Key key) : base(key) {}

        public MultiKeyGesture(Key key, ModifierKeys modifiers) : base(key, modifiers) {}

        public MultiKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : base(key, modifiers, displayString) {}

        public MultiKeyGesture(IEnumerable<Key> keys, ModifierKeys modifiers)
            : this(Key.None, modifiers)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));     
            _keys = new List<Key>(keys);
            _readOnlyKeys = new ReadOnlyCollection<Key>(_keys);
            if (_keys.Count == 0)
                throw new ArgumentException("At least one key must be specified.", nameof(keys));
        }

        public MultiKeyGesture(ICollection<(Key key, ModifierKeys modifiers)> gestureList) : base(Key.None, ModifierKeys.None)
        {
            if (gestureList == null)
                throw new ArgumentNullException(nameof(gestureList));
            if (gestureList.Count > 2)
                throw new ArgumentException($"Maximum input is 2 but given were: {gestureList.Count}");
            if (gestureList.Count == 0)
                throw new ArgumentException("At least one key must be specified.", nameof(gestureList));

            foreach (var gesturePair in gestureList)
            {
                if (gesturePair.key == Key.None)
                    throw new ArgumentException("At least one key must be specified.", nameof(gesturePair));
            }

        }

        /// <summary>
        /// All keys of the gesture
        /// </summary>
        public ICollection<Key> Keys => _readOnlyKeys;

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (_keys == null || _keys.Count <= 0)
                return base.Matches(targetElement, inputEventArgs);

            var args = inputEventArgs as KeyEventArgs;

            if (args == null || !IsDefinedKey(args.Key))
            {
                return false;
            }

            if (_currentKeyIndex != 0 && DateTime.Now - _lastKeyPress > MaximumDelayBetweenKeyPresses)
            {
                //took too long to press next key so reset
                _currentKeyIndex = 0;
                return false;
            }

            //the modifier only needs to be held down for the first keystroke, but you could also require that the modifier be held down for every keystroke
            if (_currentKeyIndex == 0 && Modifiers != Keyboard.Modifiers)
            {
                //wrong modifiers
                _currentKeyIndex = 0;
                return false;
            }

            if (_keys[_currentKeyIndex] != args.Key)
            {
                //wrong key
                _currentKeyIndex = 0;
                return false;
            }

            ++_currentKeyIndex;

            if (_currentKeyIndex != _keys.Count)
            {
                //still matching
                _lastKeyPress = DateTime.Now;
                inputEventArgs.Handled = true;
                return false;
            }

            //match complete
            _currentKeyIndex = 0;
            return true;
        }

        public static bool IsDefinedKey(Key key)
        {
            return key >= Key.None && key <= Key.OemClear;
        }
    }
}