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
            var keyStrokes = (value as string)?.Split(',');
            var firstKeyStroke = keyStrokes?[0];
            var firstKeyStrokeParts = firstKeyStroke?.Split('+');

            var modifierKeys = (ModifierKeys)_modifierKeysConverter.ConvertFrom(firstKeyStrokeParts[0]);
            var keys = new List<Key> {(Key) _keyConverter.ConvertFrom(firstKeyStrokeParts[1])};


            for (var i = 1; i < keyStrokes?.Length; ++i)
                keys.Add((Key)_keyConverter.ConvertFrom(keyStrokes[i]));

            return new MultiKeyGesture(keys, modifierKeys);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new NotSupportedException();
            
            if (!(value is MultiKeyGesture gesture))
                throw new InvalidCastException();

            var sb = new StringBuilder();
            if (gesture.GestureCollection == null || gesture.GestureCollection.Count == 0)
            {
                if (gesture.Modifiers == ModifierKeys.None)
                    return gesture.Key.ToString();
                return KeyboardLocalizationUtilities.GetCultureModifierName(gesture.Modifiers) + "+" + gesture.Key;
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
