using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

        public new string DisplayString => (string)
            KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, this,
                typeof(string));

        static MultiKeyGesture()
        {
            MaximumDelayBetweenKeyPresses = TimeSpan.FromSeconds(1);
        }

        public MultiKeyGesture(Key key) : base(key) { }

        public MultiKeyGesture(Key key, ModifierKeys modifiers) : base(key, modifiers) { }

        public MultiKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : base(key, modifiers, displayString) { }

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
                if (!gesturePair.IsValid())
                    throw new ArgumentException("Used invalid key sequence");
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

        /// <summary>
        /// Determines whether this gesture contains the specified key-sequence. 
        /// </summary>
        /// <remarks>
        /// If this instance is set <see cref="IsRealMultiKeyGesture"/> to <see langword="true"/> 
        /// it check the first specified sequence against the first sequence of this gesture.
        /// The second specified will be checked against the second sequences of this gesture. 
        /// </remarks>
        /// <param name="sequence">The sequence.</param>
        /// <param name="option">The option.</param>
        /// <returns>
        ///   <see langword="true"/> if there was a matching; <see langword="false"/> if not.
        /// </returns>
        public bool Contains(KeySequence sequence, FindKeyGestureOption option = FindKeyGestureOption.Containing)
        {
            return Contains(new List<KeySequence> { sequence }, option);
        }

        /// <summary>
        /// Determines whether this gesture contains the specified key-sequences. 
        /// </summary>
        /// <remarks>
        /// If this instance is set <see cref="IsRealMultiKeyGesture"/> to <see langword="true"/> 
        /// it check the first specified sequence against the first sequence of this gesture.
        /// The second specified will be checked against the second sequences of this gesture. 
        /// </remarks>
        /// <param name="keySequences">The key sequences.</param>
        /// <param name="option">The option.</param>
        /// <returns>
        ///   <see langword="true"/> if there was a matching; <see langword="false"/> if not.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Contains(IList<KeySequence> keySequences, FindKeyGestureOption option = FindKeyGestureOption.Containing)
        {
            if (keySequences == null)
                throw new ArgumentNullException();

            var checkCount = keySequences.Count;

            if (checkCount == 0)
                return false;
            var realSequences = GetKeySequences(this);
            var realCount = realSequences.Count;

            if (checkCount > realCount)
                return false;
            if (keySequences[0].Key != realSequences[0].Key || keySequences[0].Modifiers != realSequences[0].Modifiers)
                return false;

            if (option == FindKeyGestureOption.ExactMatch && checkCount != realCount)
                return false;
            if (checkCount < realCount || checkCount == 1)
                return true;

            if (keySequences[1].Key != realSequences[1].Key || keySequences[1].Modifiers != realSequences[1].Modifiers)
                return false;
            return false;
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

        /// <summary>
        /// Gets a list of the containing sequences even if the <see cref="IsRealMultiKeyGesture"/> is set to false;
        /// </summary>
        /// <param name="gesture">The gesture.</param>
        /// <returns>All <see cref="KeySequence"/>s found</returns>
        public static IList<KeySequence> GetKeySequences(MultiKeyGesture gesture)
        {
            if (gesture.IsRealMultiKeyGesture)
                return gesture.GestureCollection;
            if (gesture.Key == Key.None)
                return new List<KeySequence>();
            return new List<KeySequence> { new KeySequence(gesture.Modifiers, gesture.Key) };
        }
    }

    public enum FindKeyGestureOption
    {
        Containing,
        ExactMatch
    }
}