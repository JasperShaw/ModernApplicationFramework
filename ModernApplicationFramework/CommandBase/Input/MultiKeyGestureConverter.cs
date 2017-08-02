using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ModernApplicationFramework.Core.Localization;

namespace ModernApplicationFramework.CommandBase.Input
{
    /// <inheritdoc />
    /// <summary>
    /// The converter to make a string to a <see cref="MultiKeyGesture" /> and print a <see cref="MultiKeyGesture" />
    /// </summary>
    public class MultiKeyGestureConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {     
            if (!(value is string input))
                throw new InvalidCastException("Only strings can be converted to a MultiKeyGesture");
            input = input.Trim();
            if (string.IsNullOrEmpty(input))
                return new MultiKeyGesture(Key.None);
            
            var keyStrokes = input.Split(',');
            
            //Since ',' is a valid KeyGesture char we need to re-add it if it was lost by the split() operation
            for (int i = 1; i < keyStrokes.Length; i++)
                if (string.IsNullOrEmpty(keyStrokes[i]))
                    keyStrokes[i - 1] += ",";

            //Remove any possible gaps in the array
            var rawSequences = keyStrokes.Where(x => !string.IsNullOrEmpty(x)).ToList();
            var sequences = new List<KeySequence>();       
            foreach (var sequence in rawSequences)
            {
                var lastDelimiterIndex = sequence.LastIndexOf('+');
                string modifierString = string.Empty;
                string keyString;
                
                if (lastDelimiterIndex >= 0)
                {
                    if (lastDelimiterIndex == sequence.Length - 1)
                    {
                        if (sequence.Contains(KeyboardLocalizationUtilities.GetKeyCultureName(Key.OemPlus)))
                            lastDelimiterIndex = sequence.LastIndexOf('+', lastDelimiterIndex - 1);
                        else
                            lastDelimiterIndex--;
                    }
                    modifierString = sequence.Substring(0, lastDelimiterIndex).Trim();
                    keyString = sequence.Substring(lastDelimiterIndex + 1).Trim();
                }
                else
                    keyString = sequence;

                var modifiers = ModifiersFromString(modifierString, culture);
                var key = KeyFromString(keyString, culture);
                sequences.Add(new KeySequence(modifiers, key));
            }
            if (sequences.Count == 1)
                return new MultiKeyGesture(sequences[0].Key, sequences[0].Modifiers);      
            return new MultiKeyGesture(sequences);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new NotSupportedException();
            
            if (!(value is MultiKeyGesture gesture))
                throw new InvalidCastException();

            var sb = new StringBuilder();
            if (!gesture.IsRealMultiKeyGesture)
            {
                if (gesture.Modifiers == ModifierKeys.None)
                    return KeyboardLocalizationUtilities.GetKeyCultureName(gesture.Key);
                return KeyboardLocalizationUtilities.GetCultureModifiersName(gesture.Modifiers) + "+" +
                       KeyboardLocalizationUtilities.GetKeyCultureName(gesture.Key);
            }


            foreach (var i in gesture.GestureCollection)
                sb.Append($"{i}, ");

            sb.Remove(sb.Length - 2, 2);
            
            return sb.ToString();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType != typeof(string))
                return false;
            if (context?.Instance == null)
                return true;
            if (!(context.Instance is MultiKeyGesture multiKeyGesture))
                return false;
            if (multiKeyGesture.GestureCollection.Count == 0)
                return false;
            return true;
        }

        private static Key KeyFromString(string keyString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(keyString))
                return Key.None;
            return KeyboardLocalizationUtilities.CultureStringToKey(keyString.ToLower(), culture);
        }

        private static ModifierKeys ModifiersFromString(string modifierString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(modifierString))
                return ModifierKeys.None;
            var modifiers = modifierString.Split('+');
            return modifiers.Aggregate(ModifierKeys.None,
                (current, modifier) =>
                    current | KeyboardLocalizationUtilities.SingleStringToModifierKey(modifier, culture));
        }

    }
}
