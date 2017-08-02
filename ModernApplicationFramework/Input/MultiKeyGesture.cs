using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;

namespace ModernApplicationFramework.Input
{
    //http://kent-boogaart.com/blog/multikeygesture

    /// <inheritdoc />
    /// <summary>
    /// <see cref="KeyGesture"/> that allows multiple hot keys
    /// </summary>
    /// <seealso cref="KeyGesture" />
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    [ValueSerializer(typeof(MultiKeyGestureValueSerializer))]
    public class MultiKeyGesture : KeyGesture
    {
        public static TimeSpan MaximumDelayBetweenKeyPresses { get; set; }

        /// <summary>
        /// An instance of a <see cref="MultiKeyGestureConverter"/>
        /// </summary>
        public static TypeConverter KeyGestureConverter = new MultiKeyGestureConverter();


        private int _currentKeyIndex;
        private bool _isInMultiState;
        private DateTime _lastKeyPress;

        public bool WasFoundDuringMulti { get; set; }

        public IList<KeySequence> GestureCollection { get; } = new List<KeySequence>();

        /// <summary>
        /// Indicating whether this gesture is a real multi key gesture.
        /// A real multi key gesture is considered to have exactly two key sequences.
        /// <remarks>
        /// This indication is required as the property values <see cref="GestureCollection"/>, <see cref="MultiKeyGesture.Modifiers"/> and <see cref="MultiKeyGesture.Key"/>
        /// </remarks>
        /// </summary>
        public bool IsRealMultiKeyGesture => GestureCollection.Count == 2;

        static MultiKeyGesture()
        {
            MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);
        }

        public MultiKeyGesture(Key key) : base(key) { }

        public MultiKeyGesture(Key key, ModifierKeys modifiers) : base(key, modifiers) { }

        public MultiKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : base(key, modifiers, displayString) { }

        public MultiKeyGesture(IEnumerable<Key> keys, ModifierKeys modifiers)
            : this(Key.None, ModifierKeys.None)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            var keyList = keys as IList<Key> ?? keys.ToList();
            if (!keyList.Any())
                throw new ArgumentException("At least one key must be specified.", nameof(keys));
            if (keyList.Count == 1)
                throw new ArgumentException("Do not use this constructor for a normal key gesture", nameof(keys));
            if (keyList.Count > 2)
                throw new ArgumentException($"Maximum input is 2 but given were: {keyList.Count}");

            GestureCollection = new List<KeySequence>();
            var index = 0;
            foreach (var key in keyList)
            {
                GestureCollection.Add(index++ == 0
                    ? new KeySequence(modifiers, key)
                    : new KeySequence(ModifierKeys.None, key));
            }
        }

        public MultiKeyGesture(ICollection<KeySequence> gestureList) : base(Key.None, ModifierKeys.None)
        {
            if (gestureList == null)
                throw new ArgumentNullException(nameof(gestureList));
            if (gestureList.Count > 2)
                throw new ArgumentException($"Maximum input is 2 but given were: {gestureList.Count}");
            if (gestureList.Count == 1)
                throw new ArgumentException("Do not use this constructor for a normal key gesture", nameof(gestureList));
            if (gestureList.Count == 0)
                throw new ArgumentException("At least one key must be specified.", nameof(gestureList));

            GestureCollection = new List<KeySequence>();
            foreach (var gesturePair in gestureList)
            {
                if (gesturePair.Key == Key.None)
                    throw new ArgumentException("At least one key must be specified.", nameof(gesturePair));
                GestureCollection.Add(gesturePair);
            }
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (WasFoundDuringMulti)
            {
                WasFoundDuringMulti = false;
                return true;
            }

            if (!_isInMultiState && !IsRealMultiKeyGesture)
                return base.Matches(targetElement, inputEventArgs);


            if (!(inputEventArgs is KeyEventArgs args))
                return false;

            var key = args.Key != Key.System ? args.Key : args.SystemKey;

            if (!IsDefinedKey(key))
                return false;

            //Check if the key is a modifier...
            if (IsModifierKey(key))
                return false;

            //Check if the current key press happened too late...
            if (_currentKeyIndex != 0 && DateTime.Now - _lastKeyPress > MaximumDelayBetweenKeyPresses)
            {
                //The delay has expired, abort the match...
                ResetState();
                return false;
            }

            // Distinguishes between (CTRL+W, K) and (CTRL+W, CTRL+K)
            ModifierKeys toCheckModifierKeys;
            if (_currentKeyIndex != 0 && GestureCollection[_currentKeyIndex].Modifiers == ModifierKeys.None)
                toCheckModifierKeys = GestureCollection[0].Modifiers & ModifierKeys.None;
            else
                toCheckModifierKeys = GestureCollection[_currentKeyIndex].Modifiers;


            if (!Keyboard.Modifiers.HasFlag(toCheckModifierKeys))
            {
                ResetState();
                return false;
            }

            //Check if the current key is correct...
            if (GestureCollection[_currentKeyIndex].Key != key)
            {
                //The current key is not correct, abort the match...
                ResetState();
                return false;
            }

            _currentKeyIndex++;
            _isInMultiState = true;

            args.Handled = true;

            //Check if the sequence is the last one of the gesture...
            if (_currentKeyIndex != GestureCollection.Count)
            {
                //If the key is not the last one, get the current date time, handle the match event but do nothing...
                _lastKeyPress = DateTime.Now;
                inputEventArgs.Handled = true;
                return false;
            }

            args.Handled = true;
            //The gesture has finished and was correct, complete the match operation...
            ResetState();
            return true;
        }
        
        private void ResetState()
        {
            _currentKeyIndex = 0;
            _isInMultiState = false;
        }

        public static bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftAlt || key == Key.RightAlt;
        }

        public static bool IsDefinedKey(Key key)
        {
            return key >= Key.None && key <= Key.OemClear;
        }
    }
}