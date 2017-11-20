using System;
using System.Globalization;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Creators
{
    /// <inheritdoc />
    /// <summary>
    /// This implementation of <see cref="IStatusStringCreator"/> creates a localized message in the context of a given number.
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Utilities.IStatusStringCreator" />
    public class NumberStatusStringCreator : IStatusStringCreator
    {
        public string StatusTextTemplate { get; set; }
        public string PluralSuffix { get; set; }

        public NumberStatusStringCreator(string statusText, string pluralSuffix)
        {
            StatusTextTemplate = statusText;
            PluralSuffix = pluralSuffix;
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates the message 
        /// </summary>
        /// <param name="value">The number object the message should be filled with</param>
        /// <returns>
        /// Returns the message
        /// </returns>
        /// <exception cref="T:System.ArgumentException">value</exception>
        public string CreateMessage(object value)
        {
            if (!int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out int number))
                throw new ArgumentException(nameof(value));
            return Math.Abs(number) == 1 ? string.Format(StatusTextTemplate, number, string.Empty) : string.Format(StatusTextTemplate, number, PluralSuffix);
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a default message with the number 1.
        /// </summary>
        /// <returns>
        /// Returns the message
        /// </returns>
        public string CreateDefaultMessage()
        {
            return CreateMessage(1);
        }
    }
}