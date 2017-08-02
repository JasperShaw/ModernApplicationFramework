using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace ModernApplicationFramework.Input
{
    public class MultiKeyGestureValueSerializer : ValueSerializer
    {
        /// <inheritdoc />
        /// <summary>
        /// Determines whether the specified <see cref="T:System.String" /> can be converted to an instance of
        /// the type that the implementation of <see cref="T:System.Windows.Markup.ValueSerializer" /> supports.
        /// </summary>
        /// <param name="value">String to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if the value can be converted; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Determines whether the specified object can be converted into a <see cref="T:System.String" />.
        /// </summary>
        /// <param name="value">The object to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> can be converted into a 
        /// <see cref="T:System.String" />; otherwise, <see langword="false" />.
        /// </returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            var gesture = value as MultiKeyGesture;
            return gesture != null
                   && ModifierKeysConverter.IsDefinedModifierKeys(gesture.Modifiers)
                   && MultiKeyGesture.IsDefinedKey(gesture.Key);
        }

        /// <inheritdoc />
        /// <summary>
        /// Converts a <see cref="T:System.String" /> to an instance of the type that the implementation of
        /// <see cref="T:System.Windows.Markup.ValueSerializer" /> supports.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>
        /// A new instance of the type that the implementation of <see cref="T:System.Windows.Markup.ValueSerializer" />
        /// supports based on the supplied <paramref name="value" />.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="value" /> cannot be converted.
        /// </exception>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            var converter = TypeDescriptor.GetConverter(typeof(KeyGesture));
            return converter.ConvertFromString(value);
        }


        /// <inheritdoc />
        /// <summary>
        /// Converts the specified object to a <see cref="T:System.String" />.
        /// </summary>
        /// <param name="value">The object to convert into a string.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>
        /// A string representation of the specified object.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="value" /> cannot be converted.
        /// </exception>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            var converter = TypeDescriptor.GetConverter(typeof(KeyGesture));
            return converter.ConvertToInvariantString(value);
        }
    }
}
