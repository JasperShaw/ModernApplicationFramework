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
        private readonly KeyConverter _keyConverter;
        private readonly ModifierKeysConverter _modifierKeysConverter;

        public MultiKeyGestureConverter()
        {
            _keyConverter = new KeyConverter();
            _modifierKeysConverter = new ModifierKeysConverter();
        }

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
            var sequences = keyStrokes.Where(x => !string.IsNullOrEmpty(x)).ToList();

            foreach (var sequence in sequences)
            {
                var lastDelimiterIndex = sequence.LastIndexOf('+');

                if (lastDelimiterIndex >= 0)
                {
                    if (lastDelimiterIndex == sequence.Length - 1)
                        lastDelimiterIndex--;

                    var modifiers = sequence.Substring(0, lastDelimiterIndex);
                    var key = sequence.Substring(lastDelimiterIndex + 1);

                }
                else
                {
                    //Single Key without modifiers
                }
                    
            }

            //var firstKeyStroke = keyStrokes?[0];
            //var firstKeyStrokeParts = firstKeyStroke?.Split('+');

            //var modifierKeys = (ModifierKeys)_modifierKeysConverter.ConvertFrom(firstKeyStrokeParts[0]);
            //var keys = new List<Key> {(Key) _keyConverter.ConvertFrom(firstKeyStrokeParts[1])};


            //for (var i = 1; i < keyStrokes?.Length; ++i)
            //    keys.Add((Key)_keyConverter.ConvertFrom(keyStrokes[i]));

            return new MultiKeyGesture(Key.None);
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
                return KeyboardLocalizationUtilities.GetCultureModifierName(gesture.Modifiers) + "+" +
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
    }
}
