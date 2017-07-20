using System;
using System.Globalization;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Creators
{
    public class NumberStatusStringCreator : IStatusStringCreator
    {
        public string StatusTextTemplate { get; set; }
        public string PluralSuffix { get; set; }

        public NumberStatusStringCreator(string statusText, string pluralSuffix)
        {
            StatusTextTemplate = statusText;
            PluralSuffix = pluralSuffix;
        }

        public string CreateMessage(object value)
        {
            if (!int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out int number))
                throw new ArgumentException(nameof(value));
            return Math.Abs(number) == 1 ? string.Format(StatusTextTemplate, number, string.Empty) : string.Format(StatusTextTemplate, number, PluralSuffix);
        }

        public string CreateDefaultMessage()
        {
            return CreateMessage(0);
        }
    }
}